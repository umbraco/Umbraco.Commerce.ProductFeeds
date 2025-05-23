using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
