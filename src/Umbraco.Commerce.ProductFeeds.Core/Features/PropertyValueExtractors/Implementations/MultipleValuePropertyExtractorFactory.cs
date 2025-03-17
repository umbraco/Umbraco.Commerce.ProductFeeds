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
        [Obsolete("Will be removed in v15. Use TryGetExtractor instead.")]
        public IMultipleValuePropertyExtractor GetExtractor(string valueExtractorId)
        {
            if (string.IsNullOrWhiteSpace(valueExtractorId))
            {
                throw new ArgumentNullException(nameof(valueExtractorId));
            }

            IMultipleValuePropertyExtractor? valueExtractor = _valueExtractors.FirstOrDefault(x => x.Id == valueExtractorId)
                ?? throw new InvalidOperationException($"Can't find property extractor with id '{valueExtractorId}'");

            return valueExtractor;
        }

        /// <inheritdoc/>
        public bool TryGetExtractor(string valueExtractorId, out IMultipleValuePropertyExtractor? valueExtractor)
        {
            if (string.IsNullOrWhiteSpace(valueExtractorId))
            {
                throw new ArgumentNullException(nameof(valueExtractorId));
            }

            valueExtractor = _valueExtractors.FirstOrDefault(x => x.Id == valueExtractorId);
            return valueExtractor != null ? true : false;
        }
    }
}
