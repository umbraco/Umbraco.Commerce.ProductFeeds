using System.Text.Json;
using System.Xml;
using Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Application;

namespace Umbraco.Commerce.ProductFeeds.Core.Features.FeedGenerators.Application
{
    public interface IProductFeedGeneratorService
    {
        /// <summary>
        /// Returns the feed generator id. Must be unique among the feed generator services.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Returns a user friendly name of the value extractor.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Returns the feed format that this generator can generate.
        /// </summary>
        public FeedFormat Format { get; }

        Task<XmlDocument> GenerateXmlFeedAsync(ProductFeedSettingReadModel feedSetting);

        Task<JsonDocument> GenerateJsonFeedAsync(ProductFeedSettingReadModel feedSetting);
    }
}
