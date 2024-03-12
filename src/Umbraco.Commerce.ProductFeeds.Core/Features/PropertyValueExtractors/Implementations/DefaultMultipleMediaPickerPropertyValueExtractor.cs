using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Commerce.ProductFeeds.Core.Commons.Extensions;
using Umbraco.Commerce.ProductFeeds.Core.PropertyValueExtractors.Application;

namespace Umbraco.Commerce.ProductFeeds.Core.PropertyValueExtractors.Implementations
{
    public class DefaultMultipleMediaPickerPropertyValueExtractor : IMultipleValuePropertyExtractor
    {
        /// <inheritdoc/>
        public ICollection<string> Extract(IPublishedElement content, string propertyAlias, IPublishedElement? fallbackElement)
        {
            if (!content.HasProperty(propertyAlias))
            {
                return new List<string>();
            }

            List<IPublishedContent>? medias = content.GetPropertyValue<List<IPublishedContent>>(propertyAlias, fallbackElement);
            if (medias == null || medias.Count == 0)
            {
                return new List<string>();
            }

            return medias.Select(x => x.MediaUrl(mode: UrlMode.Absolute)).ToList();
        }
    }
}
