using Umbraco.Cms.Core.Models.PublishedContent;

namespace Umbraco.Commerce.ProductFeeds.Core.PropertyValueExtractors.Application
{
    /// <summary>
    /// Use to extract value of property that contains single value only.
    /// </summary>
    public interface ISingleValuePropertyExtractor
    {
        public string Name { get; }

        /// <summary>
        /// Get property value using property alias.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="propertyAlias"></param>
        /// <param name="fallbackElement">Store fallback value of the property.</param>
        /// <returns></returns>
        string Extract(IPublishedElement content, string propertyAlias, IPublishedElement? fallbackElement);
    }
}
