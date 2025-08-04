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
        public string Id => throw new NotImplementedException(); // TODO - v17: remove the default implementation.

        /// <summary>
        /// Returns a user friendly name of the value extractor.
        /// </summary>
        public string DisplayName => throw new NotImplementedException(); // TODO - v17: remove the default implementation.

        /// <summary>
        /// Returns the feed format that this generator can generate.
        /// </summary>
        public FeedFormat Format { get; }

        [Obsolete("Will be removed in v17. Use GenerateXmlFeedAsync or GenerateJsonFeedAsync instead.")]
        Task<XmlDocument> GenerateFeedAsync(ProductFeedSettingReadModel feedSetting) => GenerateXmlFeedAsync(feedSetting);

        Task<XmlDocument> GenerateXmlFeedAsync(ProductFeedSettingReadModel feedSetting) => throw new NotImplementedException("XML feed generation is not implemented.");

        Task<JsonDocument> GenerateJsonFeedAsync(ProductFeedSettingReadModel feedSetting) => throw new NotImplementedException("JSON feed generation is not implemented.");
    }
}
