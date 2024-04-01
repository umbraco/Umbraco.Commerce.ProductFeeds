using Umbraco.Commerce.Extensions;

namespace Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Application
{

#pragma warning disable CA2227 // Collection properties should be read only
    public class ProductFeedSettingReadModel
    {
        public Guid Id { get; set; }

        public ProductFeedType FeedType { get; set; }

        public string FeedTypeName => FeedType.GetDescription();

        public required string FeedName { get; set; }

        public required string FeedDescription { get; set; }

        public required Guid StoreId { get; set; }

        public Guid ProductRootKey { get; set; }

        public required IEnumerable<string> ProductDocumentTypeAliases { get; set; }

        public string? ProductChildVariantTypeAlias { get; set; }

        public required string FeedRelativePath { get; set; }

        public ICollection<PropertyValueMapping> PropertyNameMappings { get; set; } = [];
    }
#pragma warning restore CA2227 // Collection properties should be read only
}
