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
using Umbraco.Commerce.ProductFeeds.Core.Features.FeedGenerators.Implementations;
using Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Application;
using Umbraco.Commerce.ProductFeeds.Core.Features.PropertyValueExtractors.Implementations;
using Umbraco.Commerce.ProductFeeds.Core.FeedGenerators.Application;
using Umbraco.Commerce.ProductFeeds.Core.ProductQueries.Application;
using Umbraco.Commerce.ProductFeeds.Core.PropertyValueExtractors.Application;
using Umbraco.Commerce.ProductFeeds.Extensions;

namespace Umbraco.Commerce.ProductFeeds.Core.FeedGenerators.Implementations
{
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
            XmlElement titleNode = channel.OwnerDocument.CreateElement("title");
            titleNode.InnerText = feedSetting.FeedName;
            channel.AppendChild(titleNode);

            // doc/channel/description
            XmlElement descriptionNode = channel.OwnerDocument.CreateElement("description");
            descriptionNode.InnerText = feedSetting.FeedDescription;
            channel.AppendChild(descriptionNode);
            root.AppendChild(channel);

            ICollection<IPublishedContent> products = _productQueryService.GetPublishedProducts(new GetPublishedProductsParams
            {
                ProductRootKey = feedSetting.ProductRootId,
                ProductDocumentTypeIds = feedSetting.ProductDocumentTypeIds,
            });

            // render doc/channel/item nodes
            foreach (IPublishedContent product in products)
            {
                IEnumerable<IPublishedContent> childVariants = product.Children
                    .Where(x => feedSetting.ProductChildVariantTypeIds.Contains(x.ContentType.Key.ToString()))
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
                        PropertyAndNodeMapItem idPropMap = feedSetting.PropertyNameMappings.FirstOrDefault(x => x.NodeName.Equals("g:id", StringComparison.OrdinalIgnoreCase)) ?? throw new IdPropertyNodeMappingNotFoundException();
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

                        // add a url to the main product
                        AddUrlNode(itemNode, product);

                        AddPriceNode(itemNode, feedSetting, product, complexVariant.Content);

                        // group variant under the main product id
                        PropertyAndNodeMapItem idPropMap = feedSetting.PropertyNameMappings.FirstOrDefault(x => x.NodeName.Equals("g:id", StringComparison.OrdinalIgnoreCase)) ?? throw new IdPropertyNodeMappingNotFoundException();
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
        /// <param name="channel"></param>
        /// <param name="variant"></param>
        /// <param name="mainProduct"></param>
        /// <returns></returns>
        private XmlElement NewItemNode(ProductFeedSettingReadModel feedSetting, XmlElement channel, IPublishedElement variant, IPublishedElement? mainProduct)
        {
            XmlElement itemNode = channel.OwnerDocument.CreateElement("item");

            // add custom properties
            foreach (PropertyAndNodeMapItem map in feedSetting.PropertyNameMappings)
            {
                if (map.ValueExtractorName == nameof(DefaultMultipleMediaPickerPropertyValueExtractor))
                {
                    AddImageNodes(itemNode, map.ValueExtractorName, map.PropertyAlias, variant, mainProduct);
                }
                else
                {
                    ISingleValuePropertyExtractor valueExtractor = _singleValuePropertyExtractorFactory.GetExtractor(map.ValueExtractorName);
                    string propValue = valueExtractor.Extract(variant, map.PropertyAlias, mainProduct);
                    if (!string.IsNullOrWhiteSpace(propValue))
                    {
                        XmlElement propertyNode = itemNode.OwnerDocument.CreateElement(map.NodeName, GoogleXmlNamespaceUri);
                        propertyNode.InnerText = propValue;
                        itemNode.AppendChild(propertyNode);
                    }
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
            XmlElement availabilityNode = itemNode.OwnerDocument.CreateElement("g:item_group_id", GoogleXmlNamespaceUri);
            availabilityNode.InnerText = groupId;
            itemNode.AppendChild(availabilityNode);
        }

        /// <summary>
        /// Add a &lt;url&gt; node under the provided &lt;item&gt; node.
        /// </summary>
        /// <param name="itemNode"></param>
        /// <param name="product"></param>
        private static void AddUrlNode(XmlElement itemNode, IPublishedContent product)
        {
            XmlElement linkNode = itemNode.OwnerDocument.CreateElement("g:link", GoogleXmlNamespaceUri);
            linkNode.InnerText = product.Url(mode: UrlMode.Absolute);
            itemNode.AppendChild(linkNode);
        }

        /// <summary>
        /// Add a &lt;price&gt; node under the provided &lt;item&gt; node.
        /// </summary>
        /// <param name="itemNode"></param>
        /// <param name="feedSetting"></param>
        /// <param name="product"></param>
        /// <param name="complexVariant"></param>
        /// <exception cref="NotImplementedException"></exception>
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

            XmlElement priceNode = itemNode.OwnerDocument.CreateElement("g:price", GoogleXmlNamespaceUri);
            Attempt<Price> calculatePriceAttempt = productSnapshot.TryCalculatePrice();
            Price calculatedPrice = calculatePriceAttempt.Success ? calculatePriceAttempt.Result! : throw new NotImplementedException("Failed to calculate the price");
            decimal priceForShow = feedSetting.IncludeTaxInPrice ? calculatedPrice.WithTax : calculatedPrice.WithoutTax;
            priceNode.InnerText = $"{priceForShow.ToString("0.00", CultureInfo.InvariantCulture)} {_currencyService.GetCurrency(calculatedPrice.CurrencyId).Code}";
            itemNode.AppendChild(priceNode);
        }

        /// <summary>
        /// Add image nodes under the provided &lt;item&gt; node.
        /// </summary>
        /// <param name="itemNode"></param>
        /// <param name="valueExtractorName"></param>
        /// <param name="propertyAlias"></param>
        /// <param name="product"></param>
        /// <param name="mainProduct"></param>
        private void AddImageNodes(XmlElement itemNode, string valueExtractorName, string propertyAlias, IPublishedElement product, IPublishedElement? mainProduct)
        {
            IMultipleValuePropertyExtractor multipleValuePropertyExtractor = _multipleValuePropertyExtractorFactory.GetExtractor(valueExtractorName);
            var imageUrls = multipleValuePropertyExtractor.Extract(product, propertyAlias, mainProduct).ToList();
            if (imageUrls.Count <= 0)
            {
                return;
            }

            XmlElement imageNode = itemNode.OwnerDocument.CreateElement("g:image_link", GoogleXmlNamespaceUri);
            imageNode.InnerText = imageUrls.First();
            itemNode.AppendChild(imageNode);

            if (imageUrls.Count > 1)
            {
                for (int i = 1; i < imageUrls.Count; i++)
                {
                    XmlElement additionalImageNode = itemNode.OwnerDocument.CreateElement("g:additional_image_link", GoogleXmlNamespaceUri);
                    additionalImageNode.InnerText = imageUrls[i];
                    itemNode.AppendChild(additionalImageNode);
                }
            }
        }
    }
}
