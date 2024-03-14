using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Commerce.ProductFeeds.Core.Commons.Extensions;
using Umbraco.Commerce.ProductFeeds.Core.PropertyValueExtractors.Application;

namespace Umbraco.Commerce.ProductFeeds.Core.Features.PropertyValueExtractors.Implementations
{
    /// <summary>
    /// Returns either "in_stock" or "out_of_stock". If value of 'stock' property is null, still mark product as "in_stock".
    /// </summary>
    public class DefaultGoogleAvailabilityValueExtractor : ISingleValuePropertyExtractor
    {
        public string Name => nameof(DefaultGoogleAvailabilityValueExtractor);

        /// <inheritdoc/>
        public string Extract(IPublishedElement content, string propertyAlias, IPublishedElement? fallbackElement)
        {
            ArgumentNullException.ThrowIfNull(content);

            int? stock = content.GetPropertyValue<int?>(propertyAlias, fallbackElement);
            string availability = stock <= 0 ? "out_of_stock" : "in_stock"; // if 'stock' is null, still mark product in_stock

            return availability;
        }
    }
}
