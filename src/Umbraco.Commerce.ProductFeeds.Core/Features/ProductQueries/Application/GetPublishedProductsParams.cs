namespace Umbraco.Commerce.ProductFeeds.Core.ProductQueries.Application
{
    public class GetPublishedProductsParams
    {
        public int ProductRootId { get; set; }
        public required string ProductDocumentTypeAlias { get; set; }
    }
}
