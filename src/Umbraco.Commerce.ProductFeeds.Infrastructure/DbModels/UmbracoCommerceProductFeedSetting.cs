using NPoco;

namespace Umbraco.Commerce.ProductFeeds.Infrastructure.DbModels
{
    [TableName("umbracoCommerceProductFeedSetting")]
    [PrimaryKey("id", AutoIncrement = false)]
    public class UmbracoCommerceProductFeedSetting
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string FeedType { get; set; } = string.Empty;

        public string FeedRelativePath { get; set; } = string.Empty;

        public string FeedName { get; set; } = string.Empty;

        public string FeedDescription { get; set; } = string.Empty;

        public string ProductDocumentTypeAlias { get; set; } = string.Empty;

        public string ProductVariantTypeAlias { get; set; } = string.Empty;

        public string ImagesPropertyAlias { get; set; } = string.Empty;

        public Guid ProductRootId { get; set; }

        public Guid StoreId { get; set; }

        public string ProductPropertyNameMappings { get; set; } = string.Empty;
    }
}
