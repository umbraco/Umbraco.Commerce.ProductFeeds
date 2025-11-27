using System.Text.Json;
using Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Application;
using Umbraco.Commerce.ProductFeeds.Infrastructure.DbModels;

namespace Umbraco.Commerce.ProductFeeds.Infrastructure.DtoMappings
{
    /// <summary>
    /// Model mapping for ProductFeedSetting entities.
    /// Provides manual mapping between write/read models and database entities.
    /// </summary>
    public static class ProductFeedSettingMapper
    {
        /// <summary>
        /// Maps from ProductFeedSettingWriteModel to UmbracoCommerceProductFeedSetting (for database storage).
        /// This method creates a complete database entity directly from the input data without retrieving existing entities.
        /// </summary>
        /// <param name="source">The source write model</param>
        /// <returns>Mapped database entity ready for insert or update</returns>
        public static UmbracoCommerceProductFeedSetting MapToDbModel(ProductFeedSettingWriteModel source)
        {
            ArgumentNullException.ThrowIfNull(source);

            var destination = new UmbracoCommerceProductFeedSetting();

            // For edit mode, use the provided ID; for add mode, a new ID will be assigned by the service
            destination.Id = source.Id ?? Guid.Empty;
            destination.FeedRelativePath = source.FeedRelativePath;
            destination.FeedName = source.FeedName;
            destination.FeedDescription = source.FeedDescription;
            destination.StoreId = source.StoreId;
            destination.ProductRootId = source.ProductRootId;
            destination.IncludeTaxInPrice = source.IncludeTaxInPrice;

            // Parse FeedGeneratorId from string to Guid
            if (Guid.TryParse(source.FeedGeneratorId, out Guid feedGeneratorId))
            {
                destination.FeedGeneratorId = feedGeneratorId;
            }
            else
            {
                throw new ArgumentException($"Invalid FeedGeneratorId format: {source.FeedGeneratorId}", nameof(source));
            }

            // Complex mappings with serialization/joining
            destination.ProductPropertyNameMappings = JsonSerializer.Serialize(source.PropertyNameMappings);
            destination.ProductChildVariantTypeIds = string.Join(';', source.ProductChildVariantTypeIds.Select(id => id.ToString()));
            destination.ProductDocumentTypeIds = string.Join(';', source.ProductDocumentTypeIds.Select(id => id.ToString()));

            return destination;
        }

        /// <summary>
        /// Maps from UmbracoCommerceProductFeedSetting to ProductFeedSettingReadModel (for API responses).
        /// </summary>
        /// <param name="source">The source database entity</param>
        /// <returns>Mapped read model</returns>
        public static ProductFeedSettingReadModel MapToReadModel(UmbracoCommerceProductFeedSetting source)
        {
            ArgumentNullException.ThrowIfNull(source);

            return new ProductFeedSettingReadModel
            {
                Id = source.Id,
                FeedGeneratorId = source.FeedGeneratorId,
                FeedName = source.FeedName,
                FeedDescription = source.FeedDescription,
                StoreId = source.StoreId,
                ProductRootId = source.ProductRootId,
                FeedRelativePath = source.FeedRelativePath,
                IncludeTaxInPrice = source.IncludeTaxInPrice,

                // Complex mappings with deserialization/splitting
                PropertyNameMappings = DeserializePropertyMappings(source.ProductPropertyNameMappings),
                ProductChildVariantTypeIds = SplitGuidString(source.ProductChildVariantTypeIds),
                ProductDocumentTypeIds = SplitGuidString(source.ProductDocumentTypeIds)
            };
        }

        /// <summary>
        /// Helper method to deserialize property name mappings from JSON.
        /// </summary>
        private static ICollection<PropertyAndNodeMapItem> DeserializePropertyMappings(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return [];
            }

            try
            {
                return JsonSerializer.Deserialize<ICollection<PropertyAndNodeMapItem>>(json) ?? [];
            }
            catch (JsonException)
            {
                return [];
            }
        }

        /// <summary>
        /// Helper method to split semicolon-separated GUID strings into string array.
        /// </summary>
        private static string[] SplitGuidString(string guidString)
        {
            if (string.IsNullOrEmpty(guidString))
            {
                return [];
            }

            return guidString
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToArray();
        }
    }
}
