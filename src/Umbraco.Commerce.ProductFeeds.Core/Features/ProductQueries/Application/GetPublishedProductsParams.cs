namespace Umbraco.Commerce.ProductFeeds.Core.ProductQueries.Application
{
    public class GetPublishedProductsParams
    {
        public Guid ProductRootId { get; set; }
        public required IEnumerable<string> ProductDocumentTypeAliases { get; set; } = Enumerable.Empty<string>();
    }
}
