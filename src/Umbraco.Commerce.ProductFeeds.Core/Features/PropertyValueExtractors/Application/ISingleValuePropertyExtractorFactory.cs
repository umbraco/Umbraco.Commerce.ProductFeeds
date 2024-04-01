namespace Umbraco.Commerce.ProductFeeds.Core.PropertyValueExtractors.Application
{
    public interface ISingleValuePropertyExtractorFactory
    {
        /// <summary>
        /// Get the property extractor. Returns the default extractor implementation if no extractor name is provided.
        /// </summary>
        /// <param name="extractorId"></param>
        /// <returns></returns>
        ISingleValuePropertyExtractor GetExtractor(string? extractorId = null);
    }
}
