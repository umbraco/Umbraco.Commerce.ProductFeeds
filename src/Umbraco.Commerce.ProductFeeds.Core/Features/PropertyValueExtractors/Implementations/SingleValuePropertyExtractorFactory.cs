using Umbraco.Commerce.ProductFeeds.Core.PropertyValueExtractors.Application;

namespace Umbraco.Commerce.ProductFeeds.Core.Features.PropertyValueExtractors.Implementations
{
    public class SingleValuePropertyExtractorFactory : ISingleValuePropertyExtractorFactory
    {
        /// <summary>
        /// Registered single value property extractors.
        /// </summary>
        private readonly SingleValuePropertyExtractorCollection _valueExtractors;

        public SingleValuePropertyExtractorFactory(SingleValuePropertyExtractorCollection valueExtractors)
        {
            _valueExtractors = valueExtractors;
        }

        /// <inheritdoc/>
        [Obsolete("Will be removed in v15. Use TryGetExtractor instead.")]
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


        /// <inheritdoc/>
        public bool TryGetExtractor(string? extractorId, out ISingleValuePropertyExtractor? extractor)
        {
            if (string.IsNullOrWhiteSpace(extractorId))
            {
                extractorId = nameof(DefaultSingleValuePropertyExtractor);
            }

            extractor = _valueExtractors.FirstOrDefault(x => x.Id == extractorId);
            return extractor != null ? true : false;
        }
    }
}
