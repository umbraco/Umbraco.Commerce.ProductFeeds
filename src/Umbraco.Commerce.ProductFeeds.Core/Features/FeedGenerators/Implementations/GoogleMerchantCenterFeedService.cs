using System.Xml;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Commerce.Core.Api;
using Umbraco.Commerce.Core.Models;
using Umbraco.Commerce.Extensions;
using Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Application;
using Umbraco.Commerce.ProductFeeds.Core.FeedGenerators.Application;
using Umbraco.Commerce.ProductFeeds.Core.ProductQueries.Application;
using Umbraco.Commerce.ProductFeeds.Core.PropertyValueExtractors.Application;
using Umbraco.Commerce.ProductFeeds.Core.PropertyValueExtractors.Implementations;

namespace Umbraco.Commerce.ProductFeeds.Core.FeedGenerators.Implementations
{
    public class GoogleMerchantCenterFeedService : IProductFeedGeneratorService
    {
        private const string GoogleXmlNamespaceUri = "http://base.google.com/ns/1.0";
        private readonly IProductQueryService _productQueryService;
        private readonly IUmbracoCommerceApi _commerceApi;
        private readonly IUmbracoContext _umbracoContext;
        private readonly ISingleValuePropertyExtractorFactory _singleValuePropertyExtractorFactory;
        private readonly IMultipleValuePropertyExtractorFactory _multipleValuePropertyExtractorFactory;

        public GoogleMerchantCenterFeedService(
            IProductQueryService productQueryService,
            IUmbracoCommerceApi commerceApi,
            IUmbracoContextAccessor umbracoContextAccessor,
            ISingleValuePropertyExtractorFactory singleValuePropertyExtractor,
            IMultipleValuePropertyExtractorFactory multipleValuePropertyExtractorFactory)
        {
            _productQueryService = productQueryService;
            _commerceApi = commerceApi;
            _umbracoContext = umbracoContextAccessor.GetRequiredUmbracoContext();
            _singleValuePropertyExtractorFactory = singleValuePropertyExtractor;
            _multipleValuePropertyExtractorFactory = multipleValuePropertyExtractorFactory;
        }

        public XmlDocument GenerateFeed(ProductFeedSettingReadModel feedSettings)
        {
            ArgumentNullException.ThrowIfNull(feedSettings, nameof(feedSettings));

            // <doc>
            XmlDocument doc = new();
            XmlElement root = doc.CreateElement("rss");
            root.SetAttribute("xmlns:g", GoogleXmlNamespaceUri);
            root.SetAttribute("version", "2.0");
            doc.AppendChild(root);

            // doc/channel
            XmlElement channel = doc.CreateElement("channel");
            XmlElement titleNode = channel.OwnerDocument.CreateElement("title");
            titleNode.InnerText = feedSettings.FeedName;
            channel.AppendChild(titleNode);

            XmlElement descriptionNode = channel.OwnerDocument.CreateElement("description");
            descriptionNode.InnerText = feedSettings.FeedDescription;
            channel.AppendChild(descriptionNode);
            root.AppendChild(channel);

            ICollection<IPublishedContent> products = _productQueryService.GetPublishedProducts(new GetPublishedProductsParams
            {
                ProductRootId = feedSettings.ProductRootId,
                ProductDocumentTypeAlias = feedSettings.ProductDocumentTypeAlias,
            });

            // render doc/channel/item nodes
            foreach (IPublishedContent product in products)
            {
                XmlElement itemNode = channel.OwnerDocument.CreateElement("item");

                // add custom properties
                foreach (PropertyValueMapping map in feedSettings.PropertyNameMappings)
                {
                    XmlElement propertyNode = itemNode.OwnerDocument.CreateElement(map.NodeName, GoogleXmlNamespaceUri);
                    ISingleValuePropertyExtractor valueExtractor = _singleValuePropertyExtractorFactory.GetExtractor(map.ValueExtractorName);
                    propertyNode.InnerText = valueExtractor.Extract(product, map.PropertyAlias);
                    itemNode.AppendChild(propertyNode);
                }

                AddUrl(itemNode, product);
                AddPrice(itemNode, feedSettings.StoreId, product);
                AddImages(itemNode, nameof(DefaultMultipleMediaPickerPropertyValueExtractor), feedSettings.ImagesPropertyAlias, product);

                channel.AppendChild(itemNode);
            }

            return doc;
        }

        private static void AddUrl(XmlElement itemNode, IPublishedContent product)
        {
            XmlElement linkNode = itemNode.OwnerDocument.CreateElement("g:link", GoogleXmlNamespaceUri);
            linkNode.InnerText = product.Url(mode: UrlMode.Absolute);
            itemNode.AppendChild(linkNode);
        }

        private void AddPrice(XmlElement itemNode, Guid storeId, IPublishedContent product)
        {
            IProductSnapshot productSnapshot = _commerceApi.GetProduct(storeId, product.Key.ToString(), Thread.CurrentThread.CurrentCulture.Name);
            XmlElement priceNode = itemNode.OwnerDocument.CreateElement("g:price", GoogleXmlNamespaceUri);
            priceNode.InnerText = productSnapshot.CalculatePrice()?.Formatted();
            itemNode.AppendChild(priceNode);
        }

        private void AddImages(XmlElement itemNode, string valueExtractorName, string propertyAlias, IPublishedContent product)
        {
            IMultipleValuePropertyExtractor multipleValuePropertyExtractor = _multipleValuePropertyExtractorFactory.GetExtractor(valueExtractorName);
            var imageUrls = multipleValuePropertyExtractor.Extract(product, propertyAlias).ToList();
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
