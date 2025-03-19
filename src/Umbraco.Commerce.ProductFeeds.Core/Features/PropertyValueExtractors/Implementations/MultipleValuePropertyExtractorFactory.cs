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
        public bool TryGetExtractor(string valueExtractorId, out IMultipleValuePropertyExtractor? valueExtractor)
        {
            if (string.IsNullOrWhiteSpace(valueExtractorId))
            {
                throw new ArgumentNullException(nameof(valueExtractorId));
            }

            valueExtractor = _valueExtractors.FirstOrDefault(x => x.Id == valueExtractorId);
            return valueExtractor != null;
        }
    }
}
