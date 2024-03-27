using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Commerce.ProductFeeds.Extensions;
using Umbraco.Commerce.ProductFeeds.Core.PropertyValueExtractors.Application;

namespace Umbraco.Commerce.ProductFeeds.Core.Features.PropertyValueExtractors.Implementations
{
    public class DefaultMediaPickerPropertyValueExtractor : ISingleValuePropertyExtractor
    {
        public string Name => nameof(DefaultMediaPickerPropertyValueExtractor);

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
