using Umbraco.Cms.Core.Models.PublishedContent;

namespace Umbraco.Commerce.ProductFeeds.Core.PropertyValueExtractors.Application
{
    /// <summary>
    /// Use to extract value of property that contains single value only.
    /// </summary>
    public interface ISingleValuePropertyExtractor
    {
        /// <summary>
        /// Get property value using property alias.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="propertyAlias"></param>
        /// <returns></returns>
        string Extract(IPublishedContent content, string propertyAlias);
    }
}
