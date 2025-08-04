using System.Text.Json;
using System.Xml;
using Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Application;

namespace Umbraco.Commerce.ProductFeeds.Core.Features.FeedGenerators.Application
{
    public interface IProductFeedGeneratorService
    {
        /// <summary>
        /// Returns the value extractor id. Must be unique.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Returns a user friendly name of the value extractor.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Returns the feed format that this generator can generate.
        /// </summary>
        public FeedFormat Format { get; }

        Task<XmlDocument> GenerateXmlFeedAsync(ProductFeedSettingReadModel feedSetting) => throw new NotImplementedException();
        Task<JsonDocument> GenerateJsonFeedAsync(ProductFeedSettingReadModel feedSetting) => throw new NotImplementedException();
    }
}
