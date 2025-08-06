namespace Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Application
{
    public class ProductFeedSettingWriteModel
    {
        public Guid? Id { get; set; }

        public required string FeedRelativePath { get; set; }

        public required string FeedGeneratorId { get; set; }

        public required string FeedName { get; set; }

        [Obsolete("Will be removed in v17. Migrate to Feed Generator Id.")]
        public ProductFeedType? FeedType { get; set; }

        public required string FeedDescription { get; set; }

        public required Guid StoreId { get; set; }

        public Guid ProductRootId { get; set; }

        public ICollection<PropertyAndNodeMapItem> PropertyNameMappings { get; init; } = [];

        public IEnumerable<Guid> ProductChildVariantTypeIds { get; set; } = [];

        public ICollection<Guid> ProductDocumentTypeIds { get; init; } = [];

        public bool IncludeTaxInPrice { get; set; }
    }
}
