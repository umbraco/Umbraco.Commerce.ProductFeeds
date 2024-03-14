namespace Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Application
{
    public class ProductFeedSettingReadModel
    {
        public Guid Id { get; set; }

        public ProductFeedType FeedType { get; set; }

        public string FeedTypeName => FeedType.ToString();

        public required string FeedName { get; set; }

        public required string FeedDescription { get; set; }

        public required Guid StoreId { get; set; }

        public Guid ProductRootKey { get; set; }

        public required string ProductDocumentTypeAlias { get; set; }

        public Guid? ProductChildVariantTypeKey { get; set; }

        public required string FeedRelativePath { get; set; }

#pragma warning disable CA2227 // Collection properties should be read only
        public ICollection<PropertyValueMapping> PropertyNameMappings { get; set; } = new HashSet<PropertyValueMapping>();
#pragma warning restore CA2227 // Collection properties should be read only
    }
}
