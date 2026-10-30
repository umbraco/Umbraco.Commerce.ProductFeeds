using Examine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Services.Navigation;
using Umbraco.Cms.Web.Common;
using Umbraco.Commerce.ProductFeeds.Core.Features.ProductQueries.Application;

namespace Umbraco.Commerce.ProductFeeds.Core.Features.ProductQueries.Implementations
{
    public class ProductQueryService : IProductQueryService
    {
        private readonly IPublishedContentCache _publishedContentCache;
        private readonly IDocumentNavigationQueryService _navigationQueryService;
        private readonly IPublishedContentStatusFilteringService _statusFilteringService;
        private readonly IPublicAccessService _publicAccessService;
        private readonly IPublishedUrlProvider _publishedUrlProvider;
        private readonly ILogger<ProductQueryService> _logger;
        private readonly IContentTypeService _contentTypeService;

        [ActivatorUtilitiesConstructor]
        public ProductQueryService(
            IPublishedContentCache publishedContentCache,
            IDocumentNavigationQueryService navigationQueryService,
            IPublishedContentStatusFilteringService statusFilteringService,
            IPublicAccessService publicAccessService,
            IPublishedUrlProvider publishedUrlProvider,
            ILogger<ProductQueryService> logger,
            IContentTypeService contentTypeService)
        {
            _publishedContentCache = publishedContentCache;
            _navigationQueryService = navigationQueryService;
            _statusFilteringService = statusFilteringService;
            _publicAccessService = publicAccessService;
            _publishedUrlProvider = publishedUrlProvider;
            _logger = logger;
            _contentTypeService = contentTypeService;
        }

        [Obsolete("Use the constructor without IExamineManager. Will be removed in v21.0.0")]
        public ProductQueryService(
            IExamineManager examineManager,
            UmbracoHelper umbracoHelper,
            ILogger<ProductQueryService> logger,
            IContentTypeService contentTypeService)
            : this(
                StaticServiceProvider.Instance.GetRequiredService<IPublishedContentCache>(),
                StaticServiceProvider.Instance.GetRequiredService<IDocumentNavigationQueryService>(),
                StaticServiceProvider.Instance.GetRequiredService<IPublishedContentStatusFilteringService>(),
                StaticServiceProvider.Instance.GetRequiredService<IPublicAccessService>(),
                StaticServiceProvider.Instance.GetRequiredService<IPublishedUrlProvider>(),
                logger,
                contentTypeService)
        {
        }

        /// <inheritdoc/>
        public ICollection<IPublishedContent> GetPublishedProducts(GetPublishedProductsParams parameters)
        {
            ArgumentNullException.ThrowIfNull(parameters);

            IPublishedContent productRoot = _publishedContentCache.GetById(parameters.ProductRootKey)
                ?? throw new ContentNotFoundException(string.Format(null, "Unable to find product root with key = '{0}'", parameters.ProductRootKey));

            string[] productTypeAliases = [.. _contentTypeService
                .GetMany([.. parameters.ProductDocumentTypeIds.Select(idString => new Guid(idString))])
                .Select(x => x.Alias)];
            bool filterByType = parameters.ProductDocumentTypeIds.Any();

            // The old index query matched the root's own path too, so the root is a candidate as well.
            List<Guid> candidateKeys = [];
            if (!filterByType || productTypeAliases.Contains(productRoot.ContentType.Alias, StringComparer.OrdinalIgnoreCase))
            {
                candidateKeys.Add(productRoot.Key);
            }

            // Filtering by type in the navigation structure avoids materialising non-matching content.
            if (!filterByType)
            {
                if (_navigationQueryService.TryGetDescendantsKeys(productRoot.Key, out IEnumerable<Guid> descendantKeys))
                {
                    candidateKeys.AddRange(descendantKeys);
                }
            }
            else
            {
                foreach (string alias in productTypeAliases)
                {
                    if (_navigationQueryService.TryGetDescendantsKeysOfType(productRoot.Key, alias, out IEnumerable<Guid> descendantKeys))
                    {
                        candidateKeys.AddRange(descendantKeys);
                    }
                }
            }

            HashSet<int> protectedNodeIds = [.. _publicAccessService.GetAll().Select(x => x.ProtectedNodeId)];

            List<IPublishedContent> result = [];

            // Published in the current culture context (one result per node), with a published ancestor path.
            foreach (IPublishedContent content in _statusFilteringService.FilterAvailable([.. candidateKeys.Distinct()], null))
            {
                if (IsProtected(content, protectedNodeIds))
                {
                    continue;
                }

                // Exclude nodes without a routable URL (e.g. Commerce complex variant element types)
                string url = _publishedUrlProvider.GetUrl(content, UrlMode.Absolute);
                if (!string.IsNullOrEmpty(url) && url != "#")
                {
                    result.Add(content);
                }
            }

            return result;
        }

        // Protection is inherited, so a node is protected when any node in its path is.
        private static bool IsProtected(IPublishedContent content, HashSet<int> protectedNodeIds)
            => protectedNodeIds.Count > 0
                && content.Path.Split(',').Any(segment => int.TryParse(segment, out int id) && protectedNodeIds.Contains(id));
    }
}
