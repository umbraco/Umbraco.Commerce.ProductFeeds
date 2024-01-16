using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Commerce.ProductFeeds.Core.ProductQueries.Application;

namespace Umbraco.Commerce.ProductFeeds.Core.ProductQueries.Implementations
{
    public class ProductQueryService : IProductQueryService
    {
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;

        public ProductQueryService(IUmbracoContextAccessor uContextAccessor) => _umbracoContextAccessor = uContextAccessor;

        public ICollection<IPublishedContent> GetPublishedProducts(GetPublishedProductsParams parameters)
        {
            ArgumentNullException.ThrowIfNull(parameters);

            IUmbracoContext umbracoContext = _umbracoContextAccessor.GetRequiredUmbracoContext();
            IPublishedContent? productRoot = umbracoContext.Content?.GetById(parameters.ProductRootKey)
                ?? throw new InvalidOperationException($"Product root with id = {parameters.ProductRootKey} could not be found.");

            List<IPublishedContent> publishedProducts = productRoot
                .Descendants()
                .Where(x => parameters.ProductDocumentTypeAliases.Contains(x.ContentType.Alias))
                .ToList();
            return publishedProducts;
        }
    }
}
