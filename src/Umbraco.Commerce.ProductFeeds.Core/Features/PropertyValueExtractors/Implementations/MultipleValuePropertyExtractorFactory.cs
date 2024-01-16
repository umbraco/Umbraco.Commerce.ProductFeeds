using Umbraco.Commerce.ProductFeeds.Core.PropertyValueExtractors.Application;

namespace Umbraco.Commerce.ProductFeeds.Core.Features.PropertyValueExtractors.Implementations
{
    public class MultipleValuePropertyExtractorFactory : IMultipleValuePropertyExtractorFactory
    {
        private readonly MultipleValuePropertyExtractorCollection _valueExtractors;

        public MultipleValuePropertyExtractorFactory(MultipleValuePropertyExtractorCollection valueExtractors)
        {
            _valueExtractors = valueExtractors;
        }

        /// <inheritdoc/>
        public IMultipleValuePropertyExtractor GetExtractor(string uniqueExtractorName)
        {
            if (string.IsNullOrWhiteSpace(uniqueExtractorName))
            {
                throw new ArgumentNullException(nameof(uniqueExtractorName));
            }


            IMultipleValuePropertyExtractor? valueExtractor = _valueExtractors.FirstOrDefault(x => x.Name == uniqueExtractorName)
                ?? throw new InvalidOperationException($"Can't find property extractor with name '{uniqueExtractorName}'");

            return valueExtractor;
        }
    }
}
