using Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Application;

namespace Umbraco.Commerce.ProductFeeds.Core.FeedSettings.Application
{
#pragma warning disable CA2227 // Collection properties should be read only
    public class ProductFeedSettingWriteModel
    {
        public Guid? Id { get; set; }

        public required string FeedRelativePath { get; set; }

        public ProductFeedType FeedType { get; set; }

        public required string FeedName { get; set; }

        public required string FeedDescription { get; set; }

        public required Guid StoreId { get; set; }

        public Guid ProductRootId { get; set; }

        [Obsolete("Will be removed in v15. Use ProductDocumentTypeIds instead")]
        public required IEnumerable<string> ProductDocumentTypeAliases { get; set; }

        [Obsolete("Will be removed in v15. Use ProductChildVariantTypeIds instead")]
        public string? ProductChildVariantTypeAlias { get; set; }

        public ICollection<PropertyAndNodeMapItem> PropertyNameMappings { get; set; } = [];

        public IEnumerable<Guid> ProductChildVariantTypeIds { get; set; } = [];

        public ICollection<Guid> ProductDocumentTypeIds { get; set; } = [];

    }
#pragma warning restore CA2227 // Collection properties should be read only
}
