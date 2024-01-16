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

        public Guid? ProductChildVariantTypeKey { get; set; }

        public Guid ProductRootKey { get; set; }

        public Guid StoreId { get; set; }

        public string ProductPropertyNameMappings { get; set; } = string.Empty;
    }
}
