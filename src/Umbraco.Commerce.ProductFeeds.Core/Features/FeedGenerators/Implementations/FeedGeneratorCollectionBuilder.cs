using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Commerce.ProductFeeds.Core.Features.FeedGenerators.Application;

namespace Umbraco.Commerce.ProductFeeds.Core.Features.FeedGenerators.Implementations
{
    public class FeedGeneratorCollectionBuilder : OrderedCollectionBuilderBase<FeedGeneratorCollectionBuilder, FeedGeneratorCollection, IProductFeedGeneratorService>
    {
        protected override FeedGeneratorCollectionBuilder This => this;

        protected override ServiceLifetime CollectionLifetime => ServiceLifetime.Scoped;
    }
}
