using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Commerce.ProductFeeds.Core.Commons.Extensions;
using Umbraco.Commerce.ProductFeeds.Core.PropertyValueExtractors.Application;

namespace Umbraco.Commerce.ProductFeeds.Core.PropertyValueExtractors.Implementations
{
    public class DefaultGoogleAvailabilityValueExtractor : ISingleValuePropertyExtractor
    {
        /// <inheritdoc/>
        public string Extract(IPublishedContent content, string propertyAlias = "stock")
        {
            int? stock = content.GetPropertyValue<int?>(propertyAlias);
            string availability = stock <= 0 ? "out_of_stock" : "in_stock"; // if 'stock' is null, still mark product in_stock

            return availability;
        }
    }
}
