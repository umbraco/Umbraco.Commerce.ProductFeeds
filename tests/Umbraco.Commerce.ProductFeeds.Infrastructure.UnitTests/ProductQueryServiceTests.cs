using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Services.Navigation;
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
        private static readonly Guid CategoryTypeKey = new("aaaaaaaa-0000-0000-0000-000000000002");

        private readonly Dictionary<Guid, IPublishedContent> _nodes = [];
        private readonly HashSet<Guid> _published = [];
        private readonly HashSet<Guid> _nonRoutable = [];
        private readonly List<PublicAccessEntry> _publicAccessEntries = [];
        private readonly Dictionary<string, List<Guid>> _descendantsOfRoot = [];

        private readonly Mock<IPublishedContentCache> _cache = new();
        private readonly Mock<IDocumentNavigationQueryService> _navigation = new();
        private readonly Mock<IPublishedContentStatusFilteringService> _statusFiltering = new();
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
            _ = _cache.Setup(x => x.GetById(RootKey)).Returns((IPublishedContent?)null);

            _ = Assert.Throws<ContentNotFoundException>(() => sut.GetPublishedProducts(Params()));
        }

        private ProductQueryService CreateSut()
        {
            _ = _cache.Setup(x => x.GetById(It.IsAny<Guid>())).Returns((Guid key) => _nodes.GetValueOrDefault(key));

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

            _ = _statusFiltering
                .Setup(x => x.FilterAvailable(It.IsAny<IEnumerable<Guid>>(), null))
                .Returns((IEnumerable<Guid> keys, string? _) => keys.Where(_published.Contains).Select(k => _nodes[k]).ToList());

            _ = _publicAccess.Setup(x => x.GetAll()).Returns(_publicAccessEntries);

            _ = _urlProvider
                .Setup(x => x.GetUrl(It.IsAny<IPublishedContent>(), UrlMode.Absolute, null, null))
                .Returns((IPublishedContent c, UrlMode _, string? _, Uri? _) => _nonRoutable.Contains(c.Key) ? "#" : $"https://shop.test/{c.Id}");

            return new ProductQueryService(
                _cache.Object,
                _navigation.Object,
                _statusFiltering.Object,
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

        private Guid AddDescendant(int id, string alias, string parentPath, bool published = true)
        {
            var key = new Guid(id, 0, 0, [0, 0, 0, 0, 0, 0, 0, 1]);
            AddNode(key, id, alias, $"{parentPath},{id}", published);
            if (!_descendantsOfRoot.TryGetValue(alias, out List<Guid>? list))
            {
                list = [];
                _descendantsOfRoot[alias] = list;
            }

            list.Add(key);
            return key;
        }

        private void AddNode(Guid key, int id, string alias, string path, bool published)
        {
            var contentType = new Mock<IPublishedContentType>();
            _ = contentType.SetupGet(x => x.Alias).Returns(alias);

            var content = new Mock<IPublishedContent>();
            _ = content.SetupGet(x => x.Key).Returns(key);
            _ = content.SetupGet(x => x.Id).Returns(id);
            _ = content.SetupGet(x => x.Path).Returns(path);
            _ = content.SetupGet(x => x.ContentType).Returns(contentType.Object);

            _nodes[key] = content.Object;
            if (published)
            {
                _ = _published.Add(key);
            }
        }
    }
}
