using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Commerce.ProductFeeds.Core.PropertyValueExtractors.Application;
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
            if (!content.HasProperty(propertyAlias))
            {
                return new List<string>();
            }

            List<MediaWithCrops>? medias = content.GetPropertyValue<List<MediaWithCrops>>(propertyAlias, fallbackElement);
            if (medias == null || medias.Count == 0)
            {
                return new List<string>();
            }

            return medias.Select(x => x.MediaUrl(mode: UrlMode.Absolute)).ToList();
        }
    }
}
