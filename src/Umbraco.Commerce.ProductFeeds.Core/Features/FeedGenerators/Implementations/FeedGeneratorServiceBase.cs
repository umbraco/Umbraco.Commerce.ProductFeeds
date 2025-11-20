using System.Text.Json;
using System.Xml;
using Umbraco.Commerce.ProductFeeds.Core.Features.FeedGenerators.Application;
using Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Application;
using Umbraco.Commerce.ProductFeeds.Core.Features.PropertyValueExtractors.Application;

namespace Umbraco.Commerce.ProductFeeds.Core.Features.FeedGenerators.Implementations
{
    public abstract class FeedGeneratorServiceBase : IProductFeedGeneratorService
    {
        protected FeedGeneratorServiceBase(
            ISingleValuePropertyExtractorFactory singleValuePropertyExtractorFactory,
            IMultipleValuePropertyExtractorFactory multipleValuePropertyExtractorFactory)
        {
            SingleValuePropertyExtractorFactory = singleValuePropertyExtractorFactory;
            MultipleValuePropertyExtractorFactory = multipleValuePropertyExtractorFactory;
        }

        public abstract Guid Id { get; }
        public abstract string DisplayName { get; }
        public abstract FeedFormat Format { get; }

        protected ISingleValuePropertyExtractorFactory SingleValuePropertyExtractorFactory { get; }
        protected IMultipleValuePropertyExtractorFactory MultipleValuePropertyExtractorFactory { get; }

        public virtual Task<XmlDocument> GenerateXmlFeedAsync(ProductFeedSettingReadModel feedSetting) => throw new NotImplementedException("XML feed generation is not implemented.");

        public virtual Task<JsonDocument> GenerateJsonFeedAsync(ProductFeedSettingReadModel feedSetting) => throw new NotImplementedException("JSON feed generation is not implemented.");
    }
}
