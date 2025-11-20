using Umbraco.Commerce.ProductFeeds.Core.Features.FeedGenerators.Application;

namespace Umbraco.Commerce.ProductFeeds.Core.Features.FeedGenerators.Implementations
{
    internal class ProductFeedGeneratorFactory : IProductFeedGeneratorFactory
    {
        private readonly FeedGeneratorCollection _feedGenerators;

        public ProductFeedGeneratorFactory(FeedGeneratorCollection feedGenerators)
        {
            _feedGenerators = feedGenerators;
        }

        public IProductFeedGeneratorService GetGenerator(Guid feedGeneratorId)
        {
            IProductFeedGeneratorService feedGenerator = _feedGenerators.FirstOrDefault(p => p.Id == feedGeneratorId)
                ?? throw new InvalidOperationException($"Feed generator not found. id: {feedGeneratorId}");
            return feedGenerator;
        }
    }
}
