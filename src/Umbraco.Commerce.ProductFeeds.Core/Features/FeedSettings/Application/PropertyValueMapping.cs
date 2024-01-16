namespace Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Application
{
    public class PropertyValueMapping
    {
        public required string PropertyAlias { get; set; }
        public required string NodeName { get; set; }
        public string? ValueExtractorName { get; set; }
    }
}
