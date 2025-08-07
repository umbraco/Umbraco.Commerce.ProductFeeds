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

        public IProductFeedGeneratorService GetGenerator(Guid feedGeneratorId)
        {
            IProductFeedGeneratorService feedGenerator = _feedGenerators.FirstOrDefault(p => p.Id == feedGeneratorId)
                ?? throw new InvalidOperationException($"Feed generator not found. id: {feedGeneratorId}");
            return feedGenerator;
        }

        [Obsolete("Will be removed in v17. Use feedGeneratorId instead.")]
        public IProductFeedGeneratorService GetGenerator(ProductFeedType feedType)
            => GetGenerator(new Guid("101AE565-038F-443E-A29E-4FE0C7146C4A")); // Use the Google Merchant Center Feed ID as default
    }
}
