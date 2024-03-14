using Umbraco.Cms.Core.Composing;
using Umbraco.Commerce.ProductFeeds.Core.PropertyValueExtractors.Application;

namespace Umbraco.Commerce.ProductFeeds.Core.Features.PropertyValueExtractors.Implementations
{
    public class SingleValuePropertyExtractorCollectionBuilder : OrderedCollectionBuilderBase<SingleValuePropertyExtractorCollectionBuilder, SingleValuePropertyExtractorCollection, ISingleValuePropertyExtractor>
    {
        protected override SingleValuePropertyExtractorCollectionBuilder This => this;
    }
}
