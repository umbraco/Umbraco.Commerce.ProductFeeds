namespace Umbraco.Commerce.ProductFeeds.Core.ProductQueries.Application
{
    public class GetPublishedProductsParams
    {
        public Guid ProductRootId { get; set; }
        public required string ProductDocumentTypeAlias { get; set; }
    }
}
