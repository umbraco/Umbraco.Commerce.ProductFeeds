using Umbraco.Commerce.ProductFeeds.Core.PropertyValueExtractors.Application;

namespace Umbraco.Commerce.ProductFeeds.Core.Features.PropertyValueExtractors.Implementations
{
    public class SingleValuePropertyExtractorFactory : ISingleValuePropertyExtractorFactory
    {
        private readonly SingleValuePropertyExtractorCollection _valueExtractors;

        public SingleValuePropertyExtractorFactory(SingleValuePropertyExtractorCollection valueExtractors)
        {
            _valueExtractors = valueExtractors;
        }

        /// <inheritdoc/>
        public ISingleValuePropertyExtractor GetExtractor(string? uniqueExtractorName = null)
        {
            if (string.IsNullOrWhiteSpace(uniqueExtractorName))
            {
                uniqueExtractorName = nameof(DefaultSingleValuePropertyExtractor);
            }

            ISingleValuePropertyExtractor? valueExtractor = _valueExtractors.FirstOrDefault(x => x.Name == uniqueExtractorName)
                ?? throw new InvalidOperationException($"Can't find property extractor with name '{uniqueExtractorName}'");

            return valueExtractor;
        }
    }
}
