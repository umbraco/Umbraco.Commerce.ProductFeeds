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

        [Obsolete("Will be removed in v15. Use ProductDocumentTypeIds instead")]
        public string? ProductDocumentTypeAliases { get; set; }

        /// <summary>
        /// Values are separated by ';'.
        /// </summary>
        public string ProductDocumentTypeIds { get; set; } = string.Empty;

        [Obsolete("Will be removed in v15. Use ProductChildVariantTypeIds instead")]
        public string? ProductChildVariantTypeAlias { get; set; }

        /// <summary>
        /// Values are separated by ';'.
        /// </summary>
        public string ProductChildVariantTypeIds { get; set; } = string.Empty;

        public Guid ProductRootId { get; set; }

        public Guid StoreId { get; set; }

        public string ProductPropertyNameMappings { get; set; } = string.Empty;

        public bool IncludeTaxInPrice { get; set; }
    }
}
