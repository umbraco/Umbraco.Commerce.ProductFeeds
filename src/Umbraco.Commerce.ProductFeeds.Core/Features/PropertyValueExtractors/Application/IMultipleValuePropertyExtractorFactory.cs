namespace Umbraco.Commerce.ProductFeeds.Core.PropertyValueExtractors.Application
{
    public interface IMultipleValuePropertyExtractorFactory
    {
        /// <summary>
        /// Get the property extractor.
        /// </summary>
        /// <param name="valueExtractorId"></param>
        /// <returns></returns>
        IMultipleValuePropertyExtractor GetExtractor(string valueExtractorId);
    }
}
