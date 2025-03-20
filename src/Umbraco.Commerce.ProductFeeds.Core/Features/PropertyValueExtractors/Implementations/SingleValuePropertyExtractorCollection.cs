using Umbraco.Cms.Core.Composing;
using Umbraco.Commerce.ProductFeeds.Core.Features.PropertyValueExtractors.Application;

namespace Umbraco.Commerce.ProductFeeds.Core.Features.PropertyValueExtractors.Implementations
{
    public class SingleValuePropertyExtractorCollection : BuilderCollectionBase<ISingleValuePropertyExtractor>
    {
        public SingleValuePropertyExtractorCollection(Func<IEnumerable<ISingleValuePropertyExtractor>> items)
            : base(items)
        {
        }
    }
}
