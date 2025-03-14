namespace Umbraco.Commerce.ProductFeeds.Core.PropertyValueExtractors.Application
{
    public interface IMultipleValuePropertyExtractorFactory
    {
        /// <summary>
        /// Get the property extractor.
        /// </summary>
        /// <param name="valueExtractorId"></param>
        /// <returns></returns>
        [Obsolete("Will be removed in v15. Use TryGetExtractor instead.")]
        IMultipleValuePropertyExtractor GetExtractor(string valueExtractorId);

        /// <summary>
        /// Try to get the property extractor. Returns the default extractor implementation if no extractor id is provided.
        /// </summary>
        /// <param name="valueExtractorId"></param>
        /// <param name="valueExtractor">The located value extractor. It can be null if no extractor was found.</param>
        /// <returns>True if an extractor was found, otherwise False.</returns>
        bool TryGetExtractor(string valueExtractorId, out IMultipleValuePropertyExtractor? valueExtractor) => throw new NotImplementedException();
    }
}
