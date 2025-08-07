namespace Umbraco.Commerce.ProductFeeds.Core.Common.Exceptions
{
    /// <summary>
    /// This is the general exception class for the package, allowing consumers to catch exceptions thrown specifically by it.
    /// </summary>
    public class UmbracoCommerceProductFeedsGeneralException : Exception
    {
        public UmbracoCommerceProductFeedsGeneralException()
        {
        }

        public UmbracoCommerceProductFeedsGeneralException(string message)
            : base(message)
        {
        }

        public UmbracoCommerceProductFeedsGeneralException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
