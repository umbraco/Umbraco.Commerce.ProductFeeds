namespace Umbraco.Commerce.ProductFeeds.Core.Features.PropertyValueExtractors.Application
{
    public interface ISingleValuePropertyExtractorFactory
    {
        /// <summary>
        /// Try to get the property extractor. Returns the default extractor implementation if no extractor id is provided.
        /// </summary>
        /// <param name="extractorId"></param>
        /// <param name="extractor">The located value extractor. It can be null if no extractor was found.</param>
        /// <returns>True if an extractor was found, otherwise False.</returns>
        bool TryGetExtractor(string? extractorId, out ISingleValuePropertyExtractor? extractor);
    }
}
