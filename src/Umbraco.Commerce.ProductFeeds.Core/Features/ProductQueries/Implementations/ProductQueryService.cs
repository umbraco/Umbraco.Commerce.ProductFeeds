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
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;
using Umbraco.Cms.Web.Common;
using Umbraco.Commerce.ProductFeeds.Core.Features.ProductQueries.Application;

namespace Umbraco.Commerce.ProductFeeds.Core.Features.ProductQueries.Implementations
{
    public class ProductQueryService : IProductQueryService
    {
        private const int LoadBatchSize = 500;

        private readonly IPublishedContentCache _publishedContentCache;
        private readonly IDocumentNavigationQueryService _navigationQueryService;
        private readonly IDocumentPublishStatusQueryService _publishStatusQueryService;
        private readonly IDocumentCacheService _documentCacheService;
        private readonly IVariationContextAccessor _variationContextAccessor;
        private readonly IPublicAccessService _publicAccessService;
        private readonly IPublishedUrlProvider _publishedUrlProvider;
        private readonly ILogger<ProductQueryService> _logger;
        private readonly IContentTypeService _contentTypeService;

        [ActivatorUtilitiesConstructor]
        public ProductQueryService(
            IPublishedContentCache publishedContentCache,
            IDocumentNavigationQueryService navigationQueryService,
            IDocumentPublishStatusQueryService publishStatusQueryService,
            IDocumentCacheService documentCacheService,
            IVariationContextAccessor variationContextAccessor,
            IPublicAccessService publicAccessService,
            IPublishedUrlProvider publishedUrlProvider,
            ILogger<ProductQueryService> logger,
            IContentTypeService contentTypeService)
        {
            _publishedContentCache = publishedContentCache;
            _navigationQueryService = navigationQueryService;
            _publishStatusQueryService = publishStatusQueryService;
            _documentCacheService = documentCacheService;
            _variationContextAccessor = variationContextAccessor;
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
                StaticServiceProvider.Instance.GetRequiredService<IDocumentPublishStatusQueryService>(),
                StaticServiceProvider.Instance.GetRequiredService<IDocumentCacheService>(),
                StaticServiceProvider.Instance.GetRequiredService<IVariationContextAccessor>(),
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

            IPublishedContent productRoot = _publishedContentCache.GetById(false, parameters.ProductRootKey)
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

            foreach (IPublishedContent content in LoadAvailable([.. candidateKeys.Distinct()]))
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

        // Only published, never draft: a preview session on the request must not leak drafts into the feed
        // (IPublishedContentStatusFilteringService skips the publish check while previewing).
        private IEnumerable<IPublishedContent> LoadAvailable(Guid[] candidateKeys)
        {
            string culture = _variationContextAccessor.VariationContext?.Culture ?? string.Empty;

            Guid[] publishedKeys = [.. candidateKeys.Where(key =>
                _publishStatusQueryService.IsPublished(key, culture)
                && _publishStatusQueryService.HasPublishedAncestorPath(key, culture))];

            foreach (Guid[] batch in publishedKeys.Chunk(LoadBatchSize))
            {
                // Batched so a cold cache costs one round trip per batch rather than one per node.
                // The product query contract is synchronous.
                IReadOnlyList<IPublishedContent> contents = _documentCacheService.GetByKeysAsync(batch, false).GetAwaiter().GetResult();
                foreach (IPublishedContent content in contents)
                {
                    // Same rule as the CMS's FilterAvailable, plus no culture at all (outside a request) keeps the node.
                    if (culture.Length == 0
                        || culture == global::Umbraco.Cms.Core.Constants.System.InvariantCulture
                        || !content.ContentType.VariesByCulture()
                        || content.Cultures.ContainsKey(culture))
                    {
                        yield return content;
                    }
                }
            }
        }

        // Protection is inherited, so a node is protected when any node in its path is.
        private static bool IsProtected(IPublishedContent content, HashSet<int> protectedNodeIds)
            => protectedNodeIds.Count > 0
                && content.Path.Split(',').Any(segment => int.TryParse(segment, out int id) && protectedNodeIds.Contains(id));
    }
}
