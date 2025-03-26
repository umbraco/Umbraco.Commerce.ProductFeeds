using Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Application;

namespace Umbraco.Commerce.ProductFeeds.Core.Features.FeedGenerators.Application
{
    public interface IProductFeedGeneratorFactory
    {
        IProductFeedGeneratorService GetGenerator(ProductFeedType feedType);
    }
}
