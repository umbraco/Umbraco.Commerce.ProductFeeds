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
        public ISingleValuePropertyExtractor GetExtractor(string? extractorId = null)
        {
            if (string.IsNullOrWhiteSpace(extractorId))
            {
                extractorId = nameof(DefaultSingleValuePropertyExtractor);
            }

            ISingleValuePropertyExtractor? valueExtractor = _valueExtractors.FirstOrDefault(x => x.Id == extractorId)
                ?? throw new InvalidOperationException($"Can't find property extractor with id '{extractorId}'");

            return valueExtractor;
        }
    }
}
