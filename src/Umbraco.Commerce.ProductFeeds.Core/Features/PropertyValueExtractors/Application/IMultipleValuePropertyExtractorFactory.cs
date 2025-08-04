namespace Umbraco.Commerce.ProductFeeds.Core.Features.PropertyValueExtractors.Application
{
    public interface IMultipleValuePropertyExtractorFactory
    {
        /// <summary>
        /// Try to get the property extractor.
        /// </summary>
        /// <param name="valueExtractorId"></param>
        /// <param name="valueExtractor">The located value extractor. It can be null if no extractor was found.</param>
        /// <returns>True if an extractor was found, otherwise False.</returns>
        bool TryGetExtractor(string valueExtractorId, out IMultipleValuePropertyExtractor? valueExtractor);
    }
}
