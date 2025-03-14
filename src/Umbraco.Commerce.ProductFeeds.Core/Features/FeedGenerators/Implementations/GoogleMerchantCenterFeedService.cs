using System.Globalization;
using System.Xml;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Commerce.Cms.Models;
using Umbraco.Commerce.Core.Api;
using Umbraco.Commerce.Core.Models;
using Umbraco.Commerce.Core.Services;
using Umbraco.Commerce.Extensions;
using Umbraco.Commerce.ProductFeeds.Core.Extensions;
using Umbraco.Commerce.ProductFeeds.Core.Features.FeedGenerators.Implementations;
using Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Application;
using Umbraco.Commerce.ProductFeeds.Core.FeedGenerators.Application;
using Umbraco.Commerce.ProductFeeds.Core.ProductQueries.Application;
using Umbraco.Commerce.ProductFeeds.Core.PropertyValueExtractors.Application;
using Umbraco.Commerce.ProductFeeds.Extensions;

namespace Umbraco.Commerce.ProductFeeds.Core.FeedGenerators.Implementations
{
    /// <summary>
    /// This is the feed generator that follows Google Merchant Center's standard.
    /// </summary>
    public class GoogleMerchantCenterFeedService : IProductFeedGeneratorService
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
        private readonly ISingleValuePropertyExtractorFactory _singleValuePropertyExtractorFactory;
        private readonly IMultipleValuePropertyExtractorFactory _multipleValuePropertyExtractorFactory;

        public GoogleMerchantCenterFeedService(
            ILogger<GoogleMerchantCenterFeedService> logger,
            ICurrencyService currencyService,
            IProductQueryService productQueryService,
            IUmbracoCommerceApi commerceApi,
            ISingleValuePropertyExtractorFactory singleValuePropertyExtractor,
            IMultipleValuePropertyExtractorFactory multipleValuePropertyExtractorFactory)
        {
            _logger = logger;
            _currencyService = currencyService;
            _productQueryService = productQueryService;
            _commerceApi = commerceApi;
            _singleValuePropertyExtractorFactory = singleValuePropertyExtractor;
            _multipleValuePropertyExtractorFactory = multipleValuePropertyExtractorFactory;
        }

        /// <summary>
        /// Generate the product feed following the inputted settings.
        /// </summary>
        /// <param name="feedSetting"></param>
        /// <returns></returns>
        /// <exception cref="IdPropertyNodeMappingNotFoundException"></exception>
        public XmlDocument GenerateFeed(ProductFeedSettingReadModel feedSetting)
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
                ProductRootKey = feedSetting.ProductRootKey,
                ProductDocumentTypeAliases = feedSetting.ProductDocumentTypeAliases,
            });

            // render doc/channel/item nodes
            foreach (IPublishedContent product in products)
            {
                IEnumerable<IPublishedContent> childVariants = product.Children
                    .Where(x => x.ContentType.Alias == feedSetting.ProductChildVariantTypeAlias)
                    .ToList();
                if (childVariants.Any())
                {
                    // handle products with child variants
                    foreach (IPublishedContent childVariant in childVariants)
                    {
                        XmlElement itemNode = NewItemNode(feedSetting, channel, childVariant, product);

                        // add url to the main product
                        AddUrlNode(itemNode, product);

                        AddPriceNode(itemNode, feedSetting, childVariant, null);

                        // group variant into the same parent id
                        PropertyValueMapping idPropMap = feedSetting.PropertyNameMappings.FirstOrDefault(x => x.NodeName.Equals("g:id", StringComparison.OrdinalIgnoreCase)) ?? throw new IdPropertyNodeMappingNotFoundException();
                        AddItemGroupNode(itemNode, product.GetPropertyValue<object?>(idPropMap.PropertyAlias, product)?.ToString() ?? string.Empty);

                        channel.AppendChild(itemNode);
                    }
                }
                else if (product.Properties.Any(prop => prop.PropertyType.EditorAlias.Equals(Cms.Constants.PropertyEditors.Aliases.VariantsEditor, StringComparison.Ordinal)))
                {
                    // handle products with complex variants
                    IPublishedProperty complexVariantProp = product.Properties.Where(prop => prop.PropertyType.EditorAlias.Equals(Cms.Constants.PropertyEditors.Aliases.VariantsEditor, StringComparison.Ordinal)).First();
                    ProductVariantCollection variantItems = product.Value<ProductVariantCollection>(complexVariantProp.Alias)!;
                    foreach (ProductVariantItem complexVariant in variantItems)
                    {
                        XmlElement itemNode = NewItemNode(feedSetting, channel, complexVariant.Content, product);

                        // add url to the main product
                        AddUrlNode(itemNode, product);

                        AddPriceNode(itemNode, feedSetting, product, complexVariant.Content);

                        // group variant into the same parent id
                        PropertyValueMapping idPropMap = feedSetting.PropertyNameMappings.FirstOrDefault(x => x.NodeName.Equals("g:id", StringComparison.OrdinalIgnoreCase)) ?? throw new IdPropertyNodeMappingNotFoundException();
                        AddItemGroupNode(itemNode, product.GetPropertyValue<object?>(idPropMap.PropertyAlias, product)?.ToString() ?? string.Empty);

                        channel.AppendChild(itemNode);
                    }
                }
                else
                {
                    // handle product with no variants
                    XmlElement itemNode = NewItemNode(feedSetting, channel, product, null);
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
        private XmlElement NewItemNode(ProductFeedSettingReadModel feedSetting, XmlElement channel, IPublishedElement variant, IPublishedElement? mainProduct)
        {
            XmlElement itemNode = channel.OwnerDocument.CreateElement("item");

            // add custom properties
            foreach (PropertyValueMapping map in feedSetting.PropertyNameMappings)
            {
                if (_singleValuePropertyExtractorFactory.TryGetExtractor(map.ValueExtractorId, out ISingleValuePropertyExtractor? singleValueExtractor)
                    && singleValueExtractor != null)
                {
                    string propValue = singleValueExtractor.Extract(variant, map.PropertyAlias, mainProduct);
                    itemNode.AddChild(map.NodeName, propValue, GoogleXmlNamespaceUri);
                }
                else if (_multipleValuePropertyExtractorFactory.TryGetExtractor(map.ValueExtractorId!, out IMultipleValuePropertyExtractor? multipleValueExtractor)
                    && multipleValueExtractor != null)
                {
                    var values = multipleValueExtractor.Extract(variant, map.PropertyAlias, mainProduct).ToList();
                    if (map.NodeName.Equals("g:image_link", StringComparison.OrdinalIgnoreCase))
                    {
                        AddImageNodes(itemNode, values);
                    }
                    else
                    {
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
                AddPriceNode(itemNode, feedSetting, variant, null);
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
        private void AddPriceNode(XmlElement itemNode, ProductFeedSettingReadModel feedSetting, IPublishedElement product, IPublishedElement? complexVariant)
        {
            Guid storeId = feedSetting.StoreId;
            IProductSnapshot? productSnapshot = complexVariant == null ?
                _commerceApi.GetProduct(storeId, product.Key.ToString(), Thread.CurrentThread.CurrentCulture.Name)
                : _commerceApi.GetProduct(storeId, product.Key.ToString(), complexVariant.Key.ToString(), Thread.CurrentThread.CurrentCulture.Name);

            if (productSnapshot == null)
            {
                _productNotFoundLogMessage(_logger, storeId, product.Key, complexVariant?.Key, null);
                return;
            }

            Price calculatedPrice = productSnapshot.CalculatePrice();
            decimal priceForShow = feedSetting.IncludeTaxInPrice ? calculatedPrice.WithTax : calculatedPrice.WithoutTax;
            string formattedPrice = $"{priceForShow.ToString("0.00", CultureInfo.InvariantCulture)} {_currencyService.GetCurrency(calculatedPrice.CurrencyId).Code}";
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
