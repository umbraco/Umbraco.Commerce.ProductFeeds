using Microsoft.Extensions.DependencyInjection;
using Umbraco.Commerce.ProductFeeds.Core.Features.FeedGenerators.Application;
using Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Application;

namespace Umbraco.Commerce.ProductFeeds.Core.Features.FeedGenerators.Implementations
{
    // TODO - v17: make internal
    public class ProductFeedGeneratorFactory : IProductFeedGeneratorFactory
    {
        private readonly FeedGeneratorCollection _feedGenerators;

        [Obsolete("Will be removed in v17. Use the constructor that takes FeedGeneratorCollection instead.")]
        public ProductFeedGeneratorFactory(IServiceProvider serviceProvider)
            : this(serviceProvider.GetService<FeedGeneratorCollection>())
        {
        }

        public ProductFeedGeneratorFactory(FeedGeneratorCollection feedGenerators)
        {
            _feedGenerators = feedGenerators;
        }

        public IProductFeedGeneratorService GetGenerator(string feedGeneratorId)
        {
            IProductFeedGeneratorService feedGenerator = _feedGenerators.FirstOrDefault(p => p.Id == feedGeneratorId)
                ?? throw new InvalidOperationException($"Invalid generator id: {feedGeneratorId}");
            return feedGenerator;
        }

        [Obsolete("Will be removed in v17. Use feedGeneratorId instead.")]
        public IProductFeedGeneratorService GetGenerator(ProductFeedType feedType) => GetGenerator(feedType.ToString());
    }
}
