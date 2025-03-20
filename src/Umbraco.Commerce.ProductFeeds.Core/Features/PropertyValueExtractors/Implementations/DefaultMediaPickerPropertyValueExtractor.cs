using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Commerce.ProductFeeds.Extensions;
using Umbraco.Commerce.ProductFeeds.Core.Features.PropertyValueExtractors.Application;

namespace Umbraco.Commerce.ProductFeeds.Core.Features.PropertyValueExtractors.Implementations
{
    public class DefaultMediaPickerPropertyValueExtractor : ISingleValuePropertyExtractor
    {
        public string Id => nameof(DefaultMediaPickerPropertyValueExtractor);

        public string DisplayName => "Default Media Picker Property Value Extractor";

        /// <inheritdoc/>
        public string Extract(IPublishedElement content, string propertyAlias, IPublishedElement? fallbackElement)
        {
            ArgumentNullException.ThrowIfNull(content);

            IPublishedContent? media = content.GetPropertyValue<IPublishedContent>(propertyAlias, fallbackElement);
            if (media == null)
            {
                return string.Empty;
            }

            return media.MediaUrl(mode: UrlMode.Absolute);
        }
    }
}
