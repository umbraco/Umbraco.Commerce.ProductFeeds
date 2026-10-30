using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Services.Navigation;
using Umbraco.Cms.Core.Web;
using Umbraco.Commerce.ProductFeeds.Core.Features.ProductQueries.Application;
using Umbraco.Commerce.ProductFeeds.Core.Features.ProductQueries.Implementations;

namespace Umbraco.Commerce.ProductFeeds.Infrastructure.UnitTests
{
#pragma warning disable CA1515 // Consider making public types internal
    public class ProductQueryServiceTests
#pragma warning restore CA1515 // Consider making public types internal
    {
        private const int RootId = 1000;
        private static readonly Guid RootKey = new("00000000-0000-0000-0000-000000001000");
        private static readonly Guid ProductTypeKey = new("aaaaaaaa-0000-0000-0000-000000000001");

        private readonly Dictionary<Guid, IPublishedContent> _nodes = [];
        private readonly HashSet<Guid> _published = [];
        private readonly HashSet<Guid> _nonRoutable = [];
        private readonly List<PublicAccessEntry> _publicAccessEntries = [];
        private readonly Dictionary<string, List<Guid>> _descendantsOfRoot = [];

        private readonly Mock<IPublishedContentCache> _cache = new();
        private readonly Mock<IDocumentNavigationQueryService> _navigation = new();
        private readonly Mock<IDocumentPublishStatusQueryService> _publishStatus = new();
        private readonly Mock<IDocumentCacheService> _documentCache = new();
        private readonly Mock<IVariationContextAccessor> _variationContext = new();
        private readonly HashSet<Guid> _noPublishedAncestorPath = [];
        private string _currentCulture = string.Empty;
        private readonly Mock<IPublicAccessService> _publicAccess = new();
        private readonly Mock<IPublishedUrlProvider> _urlProvider = new();
        private readonly Mock<IContentTypeService> _contentTypeService = new();

        public ProductQueryServiceTests()
        {
            AddNode(RootKey, RootId, "storeRoot", "-1", published: true);
            _contentTypeService
                .Setup(x => x.GetMany(It.IsAny<IEnumerable<Guid>>()))
                .Returns((IEnumerable<Guid> keys) => keys.Select(CreateContentType).ToList());
        }

        [Fact]
        public void GetPublishedProducts_Should_Return_Descendants_Of_Root_Only()
        {
            Guid child = AddDescendant(1, "product", parentPath: "-1,1000");
            Guid grandChild = AddDescendant(2, "product", parentPath: "-1,1000,1");
            AddNode(new Guid("00000000-0000-0000-0000-00000000ffff"), 9999, "product", "-1,9999", published: true); // outside the root

            List<IPublishedContent> result = [.. CreateSut().GetPublishedProducts(Params())];

            Assert.Equal([child, grandChild], result.Select(x => x.Key).Where(x => x != RootKey).Order());
        }

        [Fact]
        public void GetPublishedProducts_Should_Include_Root_When_It_Matches()
        {
            ICollection<IPublishedContent> result = CreateSut().GetPublishedProducts(Params());

            Assert.Contains(result, x => x.Key == RootKey);
        }

        [Fact]
        public void GetPublishedProducts_Should_Only_Return_Chosen_Document_Types()
        {
            Guid product = AddDescendant(1, "product", "-1,1000");
            _ = AddDescendant(2, "category", "-1,1000");

            ICollection<IPublishedContent> result = CreateSut().GetPublishedProducts(Params(ProductTypeKey));

            Assert.Equal([product], result.Select(x => x.Key));
        }

        [Fact]
        public void GetPublishedProducts_Should_Return_All_Descendants_When_No_Document_Types_Chosen()
        {
            Guid product = AddDescendant(1, "product", "-1,1000");
            Guid category = AddDescendant(2, "category", "-1,1000");

            ICollection<IPublishedContent> result = CreateSut().GetPublishedProducts(Params());

            Assert.Equal(new[] { product, category }.Order(), result.Select(x => x.Key).Where(x => x != RootKey).Order());
        }

        [Fact]
        public void GetPublishedProducts_Should_Exclude_Unpublished_Content()
        {
            Guid published = AddDescendant(1, "product", "-1,1000");
            _ = AddDescendant(2, "product", "-1,1000", published: false);

            ICollection<IPublishedContent> result = CreateSut().GetPublishedProducts(Params(ProductTypeKey));

            Assert.Equal([published], result.Select(x => x.Key));
        }

        [Fact]
        public void GetPublishedProducts_Should_Exclude_Content_Without_Published_Ancestor_Path()
        {
            Guid ok = AddDescendant(1, "product", "-1,1000");
            Guid orphan = AddDescendant(2, "product", "-1,1000");
            _ = _noPublishedAncestorPath.Add(orphan);

            ICollection<IPublishedContent> result = CreateSut().GetPublishedProducts(Params(ProductTypeKey));

            Assert.Equal([ok], result.Select(x => x.Key));
        }

        [Fact]
        public void GetPublishedProducts_Should_Only_Load_Published_Content_Without_Preview()
        {
            _ = AddDescendant(1, "product", "-1,1000");
            _ = AddDescendant(2, "product", "-1,1000", published: false);

            _ = CreateSut().GetPublishedProducts(Params(ProductTypeKey));

            _documentCache.Verify(x => x.GetByKeysAsync(It.Is<IReadOnlyCollection<Guid>>(k => k.Count == 1), false), Times.Once);
            _documentCache.Verify(x => x.GetByKeysAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.Is<bool?>(p => p != false)), Times.Never);
        }

        [Fact]
        public void GetPublishedProducts_Should_Exclude_Variant_Content_Not_Available_In_Current_Culture()
        {
            _currentCulture = "da-DK";
            Guid invariant = AddDescendant(1, "product", "-1,1000");
            Guid availableVariant = AddDescendant(2, "product", "-1,1000", variesByCulture: true);
            _ = AddDescendant(3, "product", "-1,1000", variesByCulture: true, availableInCulture: false);

            ICollection<IPublishedContent> result = CreateSut().GetPublishedProducts(Params(ProductTypeKey));

            Assert.Equal(new[] { invariant, availableVariant }.Order(), result.Select(x => x.Key).Order());
        }

        [Fact]
        public void GetPublishedProducts_Should_Exclude_Protected_Content_And_Its_Descendants()
        {
            Guid open = AddDescendant(1, "product", "-1,1000");
            _ = AddDescendant(2, "product", "-1,1000");
            _ = AddDescendant(3, "product", "-1,1000,2");
            _publicAccessEntries.Add(new PublicAccessEntry(Guid.NewGuid(), 2, 1, 1, []));

            ICollection<IPublishedContent> result = CreateSut().GetPublishedProducts(Params(ProductTypeKey));

            Assert.Equal([open], result.Select(x => x.Key));
        }

        [Fact]
        public void GetPublishedProducts_Should_Exclude_Content_Without_Routable_Url()
        {
            Guid routable = AddDescendant(1, "product", "-1,1000");
            Guid noUrl = AddDescendant(2, "product", "-1,1000");
            _ = _nonRoutable.Add(noUrl);

            ICollection<IPublishedContent> result = CreateSut().GetPublishedProducts(Params(ProductTypeKey));

            Assert.Equal([routable], result.Select(x => x.Key));
        }

        [Fact]
        public void GetPublishedProducts_Should_Throw_When_Root_Not_Found()
        {
            ProductQueryService sut = CreateSut();
            _ = _cache.Setup(x => x.GetById(false, RootKey)).Returns((IPublishedContent?)null);

            _ = Assert.Throws<ContentNotFoundException>(() => sut.GetPublishedProducts(Params()));
        }

        private ProductQueryService CreateSut()
        {
            _ = _cache.Setup(x => x.GetById(false, It.IsAny<Guid>())).Returns((bool _, Guid key) => _nodes.GetValueOrDefault(key));

            _ = _navigation
                .Setup(x => x.TryGetDescendantsKeys(RootKey, out It.Ref<IEnumerable<Guid>>.IsAny))
                .Returns(new TryGetKeys((Guid _, out IEnumerable<Guid> keys) =>
                {
                    keys = _descendantsOfRoot.Values.SelectMany(x => x).ToList();
                    return true;
                }).Invoke);
            _ = _navigation
                .Setup(x => x.TryGetDescendantsKeysOfType(RootKey, It.IsAny<string>(), out It.Ref<IEnumerable<Guid>>.IsAny))
                .Returns(new TryGetKeysOfType((Guid _, string alias, out IEnumerable<Guid> keys) =>
                {
                    keys = _descendantsOfRoot.GetValueOrDefault(alias) is { } list ? list : [];
                    return true;
                }).Invoke);

            _ = _variationContext.SetupGet(x => x.VariationContext).Returns(() => new VariationContext(_currentCulture));

            _ = _publishStatus
                .Setup(x => x.IsPublished(It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns((Guid key, string _) => _published.Contains(key));
            _ = _publishStatus
                .Setup(x => x.HasPublishedAncestorPath(It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns((Guid key, string _) => !_noPublishedAncestorPath.Contains(key));

            _ = _documentCache
                .Setup(x => x.GetByKeysAsync(It.IsAny<IReadOnlyCollection<Guid>>(), false))
                .ReturnsAsync((IReadOnlyCollection<Guid> keys, bool? _) => (IReadOnlyList<IPublishedContent>)[.. keys.Select(k => _nodes[k])]);

            _ = _publicAccess.Setup(x => x.GetAll()).Returns(_publicAccessEntries);

            _ = _urlProvider
                .Setup(x => x.GetUrl(It.IsAny<IPublishedContent>(), UrlMode.Absolute, null, null))
                .Returns((IPublishedContent c, UrlMode _, string? _, Uri? _) => _nonRoutable.Contains(c.Key) ? "#" : $"https://shop.test/{c.Id}");

            return new ProductQueryService(
                _cache.Object,
                _navigation.Object,
                _publishStatus.Object,
                _documentCache.Object,
                _variationContext.Object,
                _publicAccess.Object,
                _urlProvider.Object,
                NullLogger<ProductQueryService>.Instance,
                _contentTypeService.Object);
        }

        private delegate bool TryGetKeys(Guid parent, out IEnumerable<Guid> keys);

        private delegate bool TryGetKeysOfType(Guid parent, string alias, out IEnumerable<Guid> keys);

        private static GetPublishedProductsParams Params(params Guid[] documentTypeIds) => new()
        {
            ProductRootKey = RootKey,
            ProductDocumentTypeIds = documentTypeIds.Select(x => x.ToString()),
        };

        private static IContentType CreateContentType(Guid key)
        {
            var type = new Mock<IContentType>();
            _ = type.SetupGet(x => x.Key).Returns(key);
            _ = type.SetupGet(x => x.Alias).Returns(key == ProductTypeKey ? "product" : "category");
            return type.Object;
        }

        private Guid AddDescendant(int id, string alias, string parentPath, bool published = true, bool variesByCulture = false, bool availableInCulture = true)
        {
            var key = new Guid(id, 0, 0, [0, 0, 0, 0, 0, 0, 0, 1]);
            AddNode(key, id, alias, $"{parentPath},{id}", published, variesByCulture, availableInCulture);
            if (!_descendantsOfRoot.TryGetValue(alias, out List<Guid>? list))
            {
                list = [];
                _descendantsOfRoot[alias] = list;
            }

            list.Add(key);
            return key;
        }

        private void AddNode(Guid key, int id, string alias, string path, bool published, bool variesByCulture = false, bool availableInCulture = true)
        {
            var contentType = new Mock<IPublishedContentType>();
            _ = contentType.SetupGet(x => x.Alias).Returns(alias);
            _ = contentType.SetupGet(x => x.Variations).Returns(variesByCulture ? ContentVariation.Culture : ContentVariation.Nothing);

            var content = new Mock<IPublishedContent>();
            _ = content.SetupGet(x => x.Key).Returns(key);
            _ = content.SetupGet(x => x.Id).Returns(id);
            _ = content.SetupGet(x => x.Path).Returns(path);
            _ = content.SetupGet(x => x.ContentType).Returns(contentType.Object);
            _ = content.SetupGet(x => x.Cultures).Returns(availableInCulture
                ? new Dictionary<string, PublishedCultureInfo> { [_currentCulture] = new(_currentCulture, "n", "/", DateTime.UtcNow) }
                : new Dictionary<string, PublishedCultureInfo>());

            _nodes[key] = content.Object;
            if (published)
            {
                _ = _published.Add(key);
            }
        }
    }
}
