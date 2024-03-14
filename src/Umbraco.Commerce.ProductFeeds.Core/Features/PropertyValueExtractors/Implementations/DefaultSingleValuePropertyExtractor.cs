using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Commerce.ProductFeeds.Core.Commons.Extensions;
using Umbraco.Commerce.ProductFeeds.Core.PropertyValueExtractors.Application;

namespace Umbraco.Commerce.ProductFeeds.Core.Features.PropertyValueExtractors.Implementations
{
    /// <summary>
    /// Simply returns .ToString() value of the property.
    /// </summary>
    public class DefaultSingleValuePropertyExtractor : ISingleValuePropertyExtractor
    {
        public string Name => nameof(DefaultSingleValuePropertyExtractor);

        /// <inheritdoc/>
        public string Extract(IPublishedElement content, string propertyAlias, IPublishedElement? fallbackElement)
        {
            ArgumentNullException.ThrowIfNull(content);

            return content.GetPropertyValue<object?>(propertyAlias, fallbackElement)?.ToString() ?? string.Empty;
        }
    }
}
