namespace Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Application
{
    public class ProductFeedSettingReadModel
    {
        public int Id { get; set; }

        public ProductFeedType FeedType { get; set; }

        public string FeedTypeName => FeedType.ToString();

        public required string FeedName { get; set; }

        public required string FeedDescription { get; set; }

        public required Guid StoreId { get; set; }

        public int ProductRootId { get; set; }

        public required string ProductDocumentTypeAlias { get; set; }

        public required string FeedRelativePath { get; set; }

        /// <summary>
        /// Gets or sets product image property alias.
        /// </summary>
        public required string ImagesPropertyAlias { get; set; }

#pragma warning disable CA2227 // Collection properties should be read only
        public ICollection<PropertyValueMapping> PropertyNameMappings { get; set; } = new HashSet<PropertyValueMapping>();
#pragma warning restore CA2227 // Collection properties should be read only
    }
}
