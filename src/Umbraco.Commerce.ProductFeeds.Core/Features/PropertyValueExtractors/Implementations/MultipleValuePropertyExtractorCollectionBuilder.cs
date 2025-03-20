using Umbraco.Cms.Core.Composing;
using Umbraco.Commerce.ProductFeeds.Core.Features.PropertyValueExtractors.Application;

namespace Umbraco.Commerce.ProductFeeds.Core.Features.PropertyValueExtractors.Implementations
{
    public class MultipleValuePropertyExtractorCollectionBuilder : OrderedCollectionBuilderBase<MultipleValuePropertyExtractorCollectionBuilder, MultipleValuePropertyExtractorCollection, IMultipleValuePropertyExtractor>
    {
        protected override MultipleValuePropertyExtractorCollectionBuilder This => this;
    }
}
