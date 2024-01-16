using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Commerce.ProductFeeds.Core.Commons.Extensions;
using Umbraco.Commerce.ProductFeeds.Core.PropertyValueExtractors.Application;

namespace Umbraco.Commerce.ProductFeeds.Core.PropertyValueExtractors.Implementations
{
    public class DefaultSingleValuePropertyExtractor : ISingleValuePropertyExtractor
    {
        /// <inheritdoc/>
        public string Extract(IPublishedContent content, string propertyAlias)
        {
            return content.GetPropertyValue<object?>(propertyAlias)?.ToString() ?? string.Empty;
        }
    }
}
