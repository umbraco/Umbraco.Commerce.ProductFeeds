using Umbraco.Commerce.Extensions;

namespace Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Application
{
    public class ProductFeedSettingReadModel
    {
        public Guid Id { get; set; }

        public required Guid FeedGeneratorId { get; set; }

        public required string FeedName { get; set; }

        [Obsolete("Will be removed in 17.")]
        public ProductFeedType FeedType { get; set; }

        [Obsolete("Will be removed in 17.")]
        public string FeedTypeName => FeedType.GetDescription();

        public required string FeedDescription { get; set; }

        public required Guid StoreId { get; set; }

        public Guid ProductRootId { get; set; }

        public IEnumerable<string> ProductDocumentTypeIds { get; set; } = [];

        public IEnumerable<string> ProductChildVariantTypeIds { get; set; } = [];

        public required string FeedRelativePath { get; set; }

        public ICollection<PropertyAndNodeMapItem> PropertyNameMappings { get; init; } = [];

        public bool IncludeTaxInPrice { get; set; }
    }
}
