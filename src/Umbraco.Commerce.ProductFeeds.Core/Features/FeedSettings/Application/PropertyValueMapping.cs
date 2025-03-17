namespace Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Application
{
    public class PropertyValueMapping
    {
        /// <summary>
        /// Product property alias.
        /// </summary>
        public required string PropertyAlias { get; set; }

        /// <summary>
        /// Xml node name.
        /// </summary>
        public required string NodeName { get; set; }

        /// <summary>
        /// Property value extractor id.
        /// </summary>
        [Obsolete("Will be removed in v15. Use ValueExtractorId instead.")]
        public string? ValueExtractorName { get; set; }

        /// <summary>
        /// Property value extractor id.
        /// </summary>
        public string? ValueExtractorId => ValueExtractorName;
    }
}
