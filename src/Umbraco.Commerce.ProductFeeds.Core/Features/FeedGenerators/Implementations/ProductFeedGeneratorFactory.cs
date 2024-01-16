using Microsoft.Extensions.DependencyInjection;
using Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Application;
using Umbraco.Commerce.ProductFeeds.Core.FeedGenerators.Application;

namespace Umbraco.Commerce.ProductFeeds.Core.FeedGenerators.Implementations
{
    public class ProductFeedGeneratorFactory : IProductFeedGeneratorFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ProductFeedGeneratorFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IProductFeedGeneratorService GetGenerator(ProductFeedType feedType)
        {
            switch (feedType)
            {
                case ProductFeedType.GoogleMerchantCenter:
                    return _serviceProvider.GetRequiredService<GoogleMerchantCenterFeedService>();

                default:
                    throw new InvalidOperationException($"Invalid feed type: {feedType}");
            }

        }
    }
}
