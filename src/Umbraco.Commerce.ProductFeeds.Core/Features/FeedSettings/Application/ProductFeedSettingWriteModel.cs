using Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Application;

namespace Umbraco.Commerce.ProductFeeds.Core.FeedSettings.Application
{
    public class ProductFeedSettingWriteModel
    {
        public Guid? Id { get; set; }

        public required string FeedRelativePath { get; set; }

        public ProductFeedType? FeedType { get; set; }

        public required string FeedName { get; set; }

        public required string FeedDescription { get; set; }

        public required Guid StoreId { get; set; }

        public Guid ProductRootId { get; set; }

        [Obsolete("Will be removed in v15. Use ProductDocumentTypeIds instead")]
        public required IEnumerable<string> ProductDocumentTypeAliases { get; set; }

        [Obsolete("Will be removed in v15. Use ProductChildVariantTypeIds instead")]
        public string? ProductChildVariantTypeAlias { get; set; }

        public ICollection<PropertyAndNodeMapItem> PropertyNameMappings { get; init; } = [];

        public IEnumerable<Guid> ProductChildVariantTypeIds { get; set; } = [];

        public ICollection<Guid> ProductDocumentTypeIds { get; init; } = [];

        public bool IncludeTaxInPrice { get; set; }
    }
}
