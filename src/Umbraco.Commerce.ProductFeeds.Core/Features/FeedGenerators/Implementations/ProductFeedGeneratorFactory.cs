using Microsoft.Extensions.DependencyInjection;
using Umbraco.Commerce.ProductFeeds.Core.Features.FeedGenerators.Application;

namespace Umbraco.Commerce.ProductFeeds.Core.Features.FeedGenerators.Implementations
{
    public class ProductFeedGeneratorFactory : IProductFeedGeneratorFactory
    {
        private readonly FeedGeneratorCollection _feedGenerators;

        public ProductFeedGeneratorFactory(FeedGeneratorCollection feedGenerators)
        {
            _feedGenerators = feedGenerators;
        }

        public IProductFeedGeneratorService GetGenerator(string feedGeneratorId)
        {
            IProductFeedGeneratorService? feedGenerator = _feedGenerators.FirstOrDefault(p => p.Id == feedGeneratorId);

            if (feedGenerator == null)
            {
                throw new InvalidOperationException($"Invalid feed id: {feedGeneratorId}");
            }
            return feedGenerator;
        }
    }
}
