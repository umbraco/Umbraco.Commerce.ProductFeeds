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

        public Guid ProductRootKey { get; set; }

        public string? ProductChildVariantTypeAlias { get; set; }

        public ICollection<string> ProductDocumentTypeAliases { get; set; } = [];

        public ICollection<PropertyValueMapping> PropertyNameMappings { get; set; } = [];

        public bool IncludeTaxInPrice { get; set; }
    }
#pragma warning restore CA2227 // Collection properties should be read only
}
