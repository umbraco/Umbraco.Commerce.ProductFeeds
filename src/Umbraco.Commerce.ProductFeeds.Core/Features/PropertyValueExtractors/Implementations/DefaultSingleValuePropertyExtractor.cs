using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Commerce.ProductFeeds.Extensions;
using Umbraco.Commerce.ProductFeeds.Core.Features.PropertyValueExtractors.Application;
using Umbraco.Extensions;

namespace Umbraco.Commerce.ProductFeeds.Core.Features.PropertyValueExtractors.Implementations
{
    /// <summary>
    /// Returns string value of the property, trims whitespace and HTML
    /// </summary>
    public class DefaultSingleValuePropertyExtractor : ISingleValuePropertyExtractor
    {
        public string Id => nameof(DefaultSingleValuePropertyExtractor);

        public string DisplayName => "Default Single Value Property Extractor";

        /// <inheritdoc/>
        public string Extract(IPublishedElement content, string propertyAlias, IPublishedElement? fallbackElement)
        {
            ArgumentNullException.ThrowIfNull(content);

            var value = content.GetPropertyValue<object?>(propertyAlias, fallbackElement)?.ToString();

            // trim whitespace
            value = value?.Trim();

            // strip HTML
            value = value?.StripHtml();

            return value ?? string.Empty;
        }
    }
}
