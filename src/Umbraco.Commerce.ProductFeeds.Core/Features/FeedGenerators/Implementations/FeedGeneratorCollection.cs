using Umbraco.Cms.Core.Composing;
using Umbraco.Commerce.ProductFeeds.Core.Features.FeedGenerators.Application;

namespace Umbraco.Commerce.ProductFeeds.Core.Features.FeedGenerators.Implementations
{
    public class FeedGeneratorCollection : BuilderCollectionBase<IProductFeedGeneratorService>
    {
        public FeedGeneratorCollection(Func<IEnumerable<IProductFeedGeneratorService>> items) : base(items)
        {
        }
    }
}
