using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Commerce.ProductFeeds.Core.Features.PropertyValueExtractors.Application;
using Umbraco.Commerce.ProductFeeds.Extensions;

namespace Umbraco.Commerce.ProductFeeds.Core.Features.PropertyValueExtractors.Implementations
{
    public class DefaultMultipleMediaPickerPropertyValueExtractor : IMultipleValuePropertyExtractor
    {
        public string Id => nameof(DefaultMultipleMediaPickerPropertyValueExtractor);

        public string DisplayName => "Default Multiple Media Picker Property Value Extractor";

        /// <inheritdoc/>
        public ICollection<string> Extract(IPublishedElement content, string propertyAlias, IPublishedElement? fallbackElement)
        {
            IEnumerable<IPublishedContent>? items = content.GetPropertyValue<IEnumerable<IPublishedContent>>(propertyAlias, fallbackElement);
            if (items == null)
            {
                return [];
            }

            return items
                .Where(x => x != null)
                .Select(x => x.MediaUrl(mode: UrlMode.Absolute))
                .ToList();
        }

    }
}
