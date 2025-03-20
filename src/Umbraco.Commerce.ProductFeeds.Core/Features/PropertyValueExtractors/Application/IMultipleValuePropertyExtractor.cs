using Umbraco.Cms.Core.Models.PublishedContent;

namespace Umbraco.Commerce.ProductFeeds.Core.Features.PropertyValueExtractors.Application
{
    /// <summary>
    /// Use to extract value of property that contains multiple value like MultipleMediaPicker.
    /// </summary>
    public interface IMultipleValuePropertyExtractor
    {
        /// <summary>
        /// The unique id of this extractor.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The user friendly name of this extractor.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Extracts the value from the property and returns them as a collection.
        /// </summary>
        /// <param name="content">The umbraco content that holds the property.</param>
        /// <param name="propertyAlias"></param>
        /// <param name="fallbackElement">If the property alias can't be found on the main content, this method should try looking for it on the fallback content.</param>
        /// <returns></returns>
        ICollection<string> Extract(IPublishedElement content, string propertyAlias, IPublishedElement? fallbackElement);
    }
}
