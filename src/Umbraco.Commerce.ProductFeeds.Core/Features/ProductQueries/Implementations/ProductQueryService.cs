using Examine;
using Examine.Search;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common;
using Umbraco.Commerce.ProductFeeds.Core.Features.ProductQueries.Application;
using Umbraco.Commerce.ProductFeeds.Core.ProductQueries.Application;

namespace Umbraco.Commerce.ProductFeeds.Core.ProductQueries.Implementations
{
    public class ProductQueryService : IProductQueryService
    {
        private readonly IExamineManager _examineManager;
        private readonly UmbracoHelper _umbracoHelper;
        private readonly ILogger<ProductQueryService> _logger;

        public ProductQueryService(
            IExamineManager examineManager,
            UmbracoHelper umbracoHelper,
            ILogger<ProductQueryService> logger)
        {
            _examineManager = examineManager;
            _umbracoHelper = umbracoHelper;
            _logger = logger;
        }

        public ICollection<IPublishedContent> GetPublishedProducts(GetPublishedProductsParams parameters)
        {
            ArgumentNullException.ThrowIfNull(parameters);

            if (!_examineManager.TryGetIndex("ExternalIndex", out IIndex? index) || index == null)
            {
                return [];
            }

            IPublishedContent? productRoot = _umbracoHelper.Content(parameters.ProductRootKey)
                    ?? throw new ContentNotFoundException(string.Format(null, "Unable to find product root with key = '{0}'", parameters.ProductRootKey));

            IBooleanOperation baseQuery = index
                .Searcher
                .CreateQuery("content")
                .Field("__Path", productRoot.Path.MultipleCharacterWildcard());

            if (parameters.ProductDocumentTypeAliases.Any())
            {
                _ = baseQuery.And(nestedQuery => nestedQuery.GroupedOr(new[] { ExamineFieldNames.ItemTypeFieldName }, parameters.ProductDocumentTypeAliases.ToArray()));
            }

            IEnumerable<string> ids = baseQuery
                .Execute()
                .Select(x => x.Id);

            List<IPublishedContent> result = new();
            foreach (string id in ids)
            {
                IPublishedContent? content = _umbracoHelper.Content(id);
                if (content != null)
                {
                    result.Add(content);
                }
            }

            return result;
        }
    }
}
