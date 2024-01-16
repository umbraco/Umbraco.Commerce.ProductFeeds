using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Commerce.ProductFeeds.Core.PropertyValueExtractors.Application;

namespace Umbraco.Commerce.ProductFeeds.Core.PropertyValueExtractors.Implementations
{
    public class DefaultMultipleMediaPickerPropertyValueExtractor : IMultipleValuePropertyExtractor
    {
        /// <inheritdoc/>
        public ICollection<string> Extract(IPublishedContent content, string propertyAlias)
        {
            if (!content.HasProperty(propertyAlias))
            {
                return new List<string>();
            }

            List<IPublishedContent>? medias = content.Value<List<IPublishedContent>>(propertyAlias);
            if (medias == null || medias.Count == 0)
            {
                return new List<string>();
            }

            return medias.Select(x => x.MediaUrl(mode: UrlMode.Absolute)).ToList();
        }
    }
}
