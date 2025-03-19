using Examine;
using Examine.Search;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
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
        private readonly IContentTypeService _contentTypeService;

        public ProductQueryService(
            IExamineManager examineManager,
            UmbracoHelper umbracoHelper,
            ILogger<ProductQueryService> logger,
            IContentTypeService contentTypeService)
        {
            _examineManager = examineManager;
            _umbracoHelper = umbracoHelper;
            _logger = logger;
            _contentTypeService = contentTypeService;
        }

        /// <inheritdoc/>
        public ICollection<IPublishedContent> GetPublishedProducts(GetPublishedProductsParams parameters)
        {
            ArgumentNullException.ThrowIfNull(parameters);

            if (!_examineManager.TryGetIndex("ExternalIndex", out IIndex? index) || index == null)
            {
                return [];
            }

            IPublishedContent? productRoot = _umbracoHelper.Content(parameters.ProductRootKey)
                    ?? throw new ContentNotFoundException(string.Format(null, "Unable to find product root with key = '{0}'", parameters.ProductRootKey));

            IEnumerable<string> productTypeAliases = _contentTypeService
                .GetMany(parameters.ProductDocumentTypeIds.Select(idString => new Guid(idString)))
                .Select(x => x.Alias);

            IBooleanOperation baseQuery = index
                .Searcher
                .CreateQuery("content")
                .Field("__Path", productRoot.Path.MultipleCharacterWildcard());

            if (parameters.ProductDocumentTypeIds.Any())
            {
                _ = baseQuery.And(nestedQuery => nestedQuery.GroupedOr(new[] { ExamineFieldNames.ItemTypeFieldName }, productTypeAliases.ToArray()));
            }

            IEnumerable<string> ids = baseQuery
                .Execute(QueryOptions.SkipTake(0, int.MaxValue))
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
