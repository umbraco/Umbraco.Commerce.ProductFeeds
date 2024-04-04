using NPoco;

namespace Umbraco.Commerce.ProductFeeds.Infrastructure.DbModels
{
    [TableName("umbracoCommerceProductFeedSetting")]
    [PrimaryKey("id", AutoIncrement = false)]
    public class UmbracoCommerceProductFeedSetting
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string FeedType { get; set; } = string.Empty;

        /// <summary>
        /// Feed URL segment.
        /// </summary>
        public string FeedRelativePath { get; set; } = string.Empty;

        public string FeedName { get; set; } = string.Empty;

        public string FeedDescription { get; set; } = string.Empty;

        /// <summary>
        /// Values are separated by ';'.
        /// </summary>
        public string ProductDocumentTypeAliases { get; set; } = string.Empty;

        public string? ProductChildVariantTypeAlias { get; set; }

        public Guid ProductRootKey { get; set; }

        public Guid StoreId { get; set; }

        public string ProductPropertyNameMappings { get; set; } = string.Empty;
    }
}
