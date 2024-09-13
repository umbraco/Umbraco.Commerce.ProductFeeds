namespace Umbraco.Commerce.ProductFeeds.Core.ProductQueries.Application
{
    public class GetPublishedProductsParams
    {
        public Guid ProductRootKey { get; set; }

        public required IEnumerable<string> ProductDocumentTypeIds { get; set; } = [];
    }
}
