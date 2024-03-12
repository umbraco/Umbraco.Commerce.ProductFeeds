using Umbraco.Cms.Core.Models.PublishedContent;

namespace Umbraco.Commerce.ProductFeeds.Core.PropertyValueExtractors.Application
{
    /// <summary>
    /// Use to extract value of property that contains multiple value like MultipleMediaPicker.
    /// </summary>
    public interface IMultipleValuePropertyExtractor
    {
        /// <summary>
        /// Get a list of absolute url of the media.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="propertyAlias"></param>
        /// <param name="fallbackElement"></param>
        /// <returns></returns>
        ICollection<string> Extract(IPublishedElement content, string propertyAlias, IPublishedElement? fallbackElement);
    }
}
