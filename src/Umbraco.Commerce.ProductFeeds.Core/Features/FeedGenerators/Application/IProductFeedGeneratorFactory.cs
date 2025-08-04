namespace Umbraco.Commerce.ProductFeeds.Core.Features.FeedGenerators.Application
{
    public interface IProductFeedGeneratorFactory
    {
        IProductFeedGeneratorService GetGenerator(string feedGeneratorId);
    }
}
