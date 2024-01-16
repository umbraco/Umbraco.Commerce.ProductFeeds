

namespace Umbraco.Commerce.ProductFeeds.Infrastructure.DbModels
{
    //[TableName("umbracoCommerceProductFeedSetting")]
    //[PrimaryKey("id", AutoIncrement = true)]
    //[ExplicitColumns]
    public class UmbracoCommerceProductFeedSetting
    {
        //[PrimaryKeyColumn(AutoIncrement = true, IdentitySeed = 1)]
        //[Column("id")]
        public int Id { get; set; }

        //[Column("feedType")]
        public required string FeedType { get; set; }

        //[Column("feedRelativePath")]
        public required string FeedRelativePath { get; set; }
        public required string FeedName { get; set; }
        public required string FeedDescription { get; set; }
        public required string ProductDocumentTypeAlias { get; set; }
        public required string ImagesPropertyAlias { get; set; }
        public required int ProductRootId { get; set; }

        public required Guid StoreId { get; set; }
        public required string ProductPropertyNameMappings { get; set; }
    }
}
