namespace Umbraco.Commerce.ProductFeeds.Core.PropertyValueExtractors.Application
{
    public interface IMultipleValuePropertyExtractorFactory
    {
        /// <summary>
        /// Get the property extractor.
        /// </summary>
        /// <param name="uniqueExtractorName"></param>
        /// <returns></returns>
        IMultipleValuePropertyExtractor GetExtractor(string uniqueExtractorName);
    }
}
