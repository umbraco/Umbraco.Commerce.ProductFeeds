using System.Globalization;
using System.Xml;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Commerce.Cms.Models;
using Umbraco.Commerce.Common.Models;
using Umbraco.Commerce.Core.Api;
using Umbraco.Commerce.Core.Models;
using Umbraco.Commerce.Core.Services;
using Umbraco.Commerce.Extensions;
using Umbraco.Commerce.ProductFeeds.Core.Extensions;
using Umbraco.Commerce.ProductFeeds.Core.Features.FeedGenerators.Application;
using Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Application;
using Umbraco.Commerce.ProductFeeds.Core.Features.ProductQueries.Application;
using Umbraco.Commerce.ProductFeeds.Core.Features.PropertyValueExtractors.Application;
using Umbraco.Commerce.ProductFeeds.Extensions;

namespace Umbraco.Commerce.ProductFeeds.Core.Features.FeedGenerators.Implementations
{
    /// <summary>
    /// This is the feed generator that follows Google Merchant Center's standard.
    /// </summary>
    public class GoogleMerchantCenterFeedService : FeedGeneratorServiceBase
    {
        private const string GoogleXmlNamespaceUri = "http://base.google.com/ns/1.0";

        private static readonly Action<ILogger, Guid, Guid, Guid?, Exception?> _productNotFoundLogMessage = LoggerMessage.Define<Guid, Guid, Guid?>(
            LogLevel.Error,
            0,
            "Unable to find any product with these parameters: storeId = '{StoreId}', product key= '{ProductKey}', variant key = '{VariantKey}'.");

        private readonly ILogger<GoogleMerchantCenterFeedService> _logger;
        private readonly ICurrencyService _currencyService;
        private readonly IProductQueryService _productQueryService;
        private readonly IUmbracoCommerceApi _commerceApi;

        public override string Id => "GoogleMerchantCenter";

        public override string DisplayName => "Google Merchant Center Feed";

        public override FeedFormat Format => FeedFormat.Xml;

        public GoogleMerchantCenterFeedService(
            ILogger<GoogleMerchantCenterFeedService> logger,
            ICurrencyService currencyService,
            IProductQueryService productQueryService,
            IUmbracoCommerceApi commerceApi,
            ISingleValuePropertyExtractorFactory singleValuePropertyExtractorFactory,
            IMultipleValuePropertyExtractorFactory multipleValuePropertyExtractorFactory)
            : base(singleValuePropertyExtractorFactory, multipleValuePropertyExtractorFactory)
        {
            _logger = logger;
            _currencyService = currencyService;
            _productQueryService = productQueryService;
            _commerceApi = commerceApi;
        }

        /// <summary>
        /// Generate the product feed following the inputted settings.
        /// </summary>
        /// <param name="feedSetting"></param>
        /// <returns></returns>
        /// <exception cref="IdPropertyNodeMappingNotFoundException"></exception>
        public override async Task<XmlDocument> GenerateXmlFeedAsync(ProductFeedSettingReadModel feedSetting)
        {
            ArgumentNullException.ThrowIfNull(feedSetting, nameof(feedSetting));

            // <doc>
            XmlDocument doc = new();
            XmlElement root = doc.CreateElement("rss");
            root.SetAttribute("xmlns:g", GoogleXmlNamespaceUri);
            root.SetAttribute("version", "2.0");
            doc.AppendChild(root);

            // doc/channel
            XmlElement channel = doc.CreateElement("channel");

            // doc/channel/title
            channel.AddChild("title", feedSetting.FeedName);

            // doc/channel/description
            channel.AddChild("description", feedSetting.FeedDescription);
            root.AppendChild(channel);

            ICollection<IPublishedContent> products = _productQueryService.GetPublishedProducts(new GetPublishedProductsParams
            {
                ProductRootKey = feedSetting.ProductRootId,
                ProductDocumentTypeIds = feedSetting.ProductDocumentTypeIds,
            });

            // render doc/channel/item nodes
            foreach (IPublishedContent product in products)
            {
                IEnumerable<IPublishedContent> childVariants = product.Children()
                    .Where(x => feedSetting.ProductChildVariantTypeIds.Contains(x.ContentType.Key.ToString()))
                    .ToList();
                if (childVariants.Any())
                {
                    // handle products with child variants
                    foreach (IPublishedContent childVariant in childVariants)
                    {
                        XmlElement itemNode = await NewItemNodeAsync(feedSetting, channel, childVariant, product);

                        // add url to the main product
                        AddUrlNode(itemNode, product);

                        await AddPriceNodeAsync(itemNode, feedSetting, childVariant, null);

                        // group variant into the same parent id
                        PropertyAndNodeMapItem idPropMap = feedSetting.PropertyNameMappings.FirstOrDefault(x => x.NodeName.Equals("g:id", StringComparison.OrdinalIgnoreCase)) ?? throw new IdPropertyNodeMappingNotFoundException();
                        AddItemGroupNode(itemNode, product.GetPropertyValue<object?>(idPropMap.PropertyAlias, product)?.ToString() ?? string.Empty);

                        channel.AppendChild(itemNode);
                    }
                }
                else if (product.Properties.Any(prop => prop.PropertyType.EditorAlias.Equals(Cms.Constants.PropertyEditors.Aliases.VariantsEditor, StringComparison.Ordinal)))
                {
                    // handle products with complex variants
                    IPublishedProperty complexVariantProp = product.Properties.First(prop => prop.PropertyType.EditorAlias.Equals(Cms.Constants.PropertyEditors.Aliases.VariantsEditor, StringComparison.Ordinal));
                    ProductVariantCollection variantItems = product.Value<ProductVariantCollection>(complexVariantProp.Alias)!;
                    foreach (ProductVariantItem complexVariant in variantItems)
                    {
                        XmlElement itemNode = await NewItemNodeAsync(feedSetting, channel, complexVariant.Content, product);

                        // add a url to the main product
                        AddUrlNode(itemNode, product);

                        await AddPriceNodeAsync(itemNode, feedSetting, product, complexVariant.Content);

                        // group variant under the main product id
                        PropertyAndNodeMapItem idPropMap = feedSetting.PropertyNameMappings.FirstOrDefault(x => x.NodeName.Equals("g:id", StringComparison.OrdinalIgnoreCase)) ?? throw new IdPropertyNodeMappingNotFoundException();
                        AddItemGroupNode(itemNode, product.GetPropertyValue<object?>(idPropMap.PropertyAlias, product)?.ToString() ?? string.Empty);

                        channel.AppendChild(itemNode);
                    }
                }
                else
                {
                    // handle product with no variants
                    XmlElement itemNode = await NewItemNodeAsync(feedSetting, channel, product, null);
                    channel.AppendChild(itemNode);
                }
            }

            return doc;
        }

        /// <summary>
        /// Create a new node for the one, in this case, each product/variant is one &lt;item&gt; node.
        /// </summary>
        /// <param name="feedSetting"></param>
        /// <param name="channel">The XML &lt;channel&gt; node that holds the &lt;item&gt; node.</param>
        /// <param name="variant">Product variant.</param>
        /// <param name="mainProduct">Main product which may have multiple variants. Some common properties can only be found in the main product, not in a variant.</param>
        /// <returns></returns>
        private async Task<XmlElement> NewItemNodeAsync(ProductFeedSettingReadModel feedSetting, XmlElement channel, IPublishedElement variant, IPublishedElement? mainProduct)
        {
            XmlElement itemNode = channel.OwnerDocument.CreateElement("item");

            // add custom properties
            foreach (PropertyAndNodeMapItem map in feedSetting.PropertyNameMappings)
            {
                if (SingleValuePropertyExtractorFactory.TryGetExtractor(map.ValueExtractorId, out ISingleValuePropertyExtractor? singleValueExtractor)
                    && singleValueExtractor != null)
                {
                    string propValue = singleValueExtractor.Extract(variant, map.PropertyAlias, mainProduct);
                    itemNode.AddChild(map.NodeName, propValue, GoogleXmlNamespaceUri);
                }
                else if (MultipleValuePropertyExtractorFactory.TryGetExtractor(map.ValueExtractorId!, out IMultipleValuePropertyExtractor? multipleValueExtractor)
                    && multipleValueExtractor != null)
                {
                    var values = multipleValueExtractor.Extract(variant, map.PropertyAlias, mainProduct).ToList();
                    if (map.NodeName.Equals("g:image_link", StringComparison.OrdinalIgnoreCase))
                    {
                        // image nodes are special, they can have multiple values, but Google Merchant Center treats them differently
                        AddImageNodes(itemNode, values);
                    }
                    else
                    {
                        // handle general multiple value properties
                        foreach (string value in values)
                        {
                            itemNode.AddChild(map.NodeName, value, GoogleXmlNamespaceUri);
                        }
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Unable to locate property value extractor with id = '{map.ValueExtractorId}'");
                }
            }

            if (mainProduct == null) // when the variant is the main product itself
            {
                AddUrlNode(itemNode, (IPublishedContent)variant);
                await AddPriceNodeAsync(itemNode, feedSetting, variant, null);
            }

            return itemNode;
        }

        private static void AddItemGroupNode(XmlElement itemNode, string groupId)
        {
            itemNode.AddChild("g:item_group_id", groupId, GoogleXmlNamespaceUri);
        }

        /// <summary>
        /// Add a &lt;url&gt; node under the provided &lt;item&gt; node.
        /// </summary>
        /// <param name="itemNode">The XML item node that represents the current product.</param>
        /// <param name="product"></param>
        private static void AddUrlNode(XmlElement itemNode, IPublishedContent product)
        {
            itemNode.AddChild("g:link", product.Url(mode: UrlMode.Absolute), GoogleXmlNamespaceUri);
        }

        /// <summary>
        /// Add a &lt;price&gt; node under the provided &lt;item&gt; node.
        /// </summary>
        /// <param name="itemNode">The XML item node that represents the current product.</param>
        /// <param name="feedSetting"></param>
        /// <param name="product"></param>
        /// <param name="complexVariant"></param>
        /// <exception cref="NotImplementedException"></exception>
        private async Task AddPriceNodeAsync(XmlElement itemNode, ProductFeedSettingReadModel feedSetting, IPublishedElement product, IPublishedElement? complexVariant)
        {
            Guid storeId = feedSetting.StoreId;
            IProductSnapshot? productSnapshot = complexVariant == null ?
                await _commerceApi.GetProductAsync(storeId, product.Key.ToString(), Thread.CurrentThread.CurrentCulture.Name)
                : await _commerceApi.GetProductAsync(storeId, product.Key.ToString(), complexVariant.Key.ToString(), Thread.CurrentThread.CurrentCulture.Name);

            if (productSnapshot == null)
            {
                _productNotFoundLogMessage(_logger, storeId, product.Key, complexVariant?.Key, null);
                return;
            }

            Attempt<Price> calculatePriceAttempt = await productSnapshot.TryCalculatePriceAsync();
            Price calculatedPrice = calculatePriceAttempt.Success ? calculatePriceAttempt.Result! : throw new NotImplementedException("Failed to calculate the price");
            decimal priceForShow = feedSetting.IncludeTaxInPrice ? calculatedPrice.WithTax : calculatedPrice.WithoutTax;
            string formattedPrice = $"{priceForShow.ToString("0.00", CultureInfo.InvariantCulture)} {(await _currencyService.GetCurrencyAsync(calculatedPrice.CurrencyId)).Code}";
            itemNode.AddChild("g:price", formattedPrice, GoogleXmlNamespaceUri);
        }

        /// <summary>
        /// This method adds appropriate image nodes under the provided &lt;item&gt; node. It exists because Google Merchant Center treats multiple product images abnormally.
        /// </summary>
        /// <param name="itemNode">The XML item node that represents the current product.</param>
        /// <param name="imageUrls"></param>
        private static void AddImageNodes(XmlElement itemNode, List<string> imageUrls)
        {
            if (imageUrls.Count == 0)
            {
                return;
            }

            itemNode.AddChild("g:image_link", imageUrls.First(), GoogleXmlNamespaceUri);
            if (imageUrls.Count > 1)
            {
                for (int i = 1; i < imageUrls.Count; i++)
                {
                    itemNode.AddChild("g:additional_image_link", imageUrls[i], GoogleXmlNamespaceUri);
                }
            }
        }
    }
}
