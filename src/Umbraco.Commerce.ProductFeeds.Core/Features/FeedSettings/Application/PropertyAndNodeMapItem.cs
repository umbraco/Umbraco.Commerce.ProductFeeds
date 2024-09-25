namespace Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Application
{
    public class PropertyAndNodeMapItem
    {
        public required string PropertyAlias { get; set; }
        public required string NodeName { get; set; }

        /// <summary>
        /// Property value extractor id.
        /// </summary>
        public string? ValueExtractorName { get; set; }
    }
}
