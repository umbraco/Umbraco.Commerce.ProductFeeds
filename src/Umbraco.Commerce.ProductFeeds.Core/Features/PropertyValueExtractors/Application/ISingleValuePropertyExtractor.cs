using Umbraco.Cms.Core.Models.PublishedContent;

namespace Umbraco.Commerce.ProductFeeds.Core.Features.PropertyValueExtractors.Application
{
    /// <summary>
    /// Use to extract value of property that contains single value only.
    /// </summary>
    public interface ISingleValuePropertyExtractor
    {
        /// <summary>
        /// Returns the value extractor id. Must be unique.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Returns a user friendly name of the value extractor.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Gets property value using property alias.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="propertyAlias"></param>
        /// <param name="fallbackElement">Stores fallback value of the property.</param>
        /// <returns></returns>
        string Extract(IPublishedElement content, string propertyAlias, IPublishedElement? fallbackElement);
    }
}
