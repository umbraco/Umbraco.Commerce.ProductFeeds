using Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Application;

namespace Umbraco.Commerce.ProductFeeds.Core.Features.FeedGenerators.Application
{
    public interface IProductFeedGeneratorFactory
    {
        [Obsolete("Will be removed in v17. Use the overload that takes feedGeneratorId.")]
        IProductFeedGeneratorService GetGenerator(ProductFeedType feedType)
            => GetGenerator(feedType.ToString());

        // TODO - v17: remove the default implementation
        IProductFeedGeneratorService GetGenerator(string feedGeneratorId)
            => throw new NotImplementedException("This method should be implemented in the concrete factory class.");
    }
}
