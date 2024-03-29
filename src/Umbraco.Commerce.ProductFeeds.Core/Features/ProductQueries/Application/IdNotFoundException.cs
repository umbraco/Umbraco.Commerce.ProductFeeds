namespace Umbraco.Commerce.ProductFeeds.Core.Features.ProductQueries.Application
{
    [Serializable]
    public class IdNotFoundException : Exception
    {
        public IdNotFoundException()
        {
        }

        public IdNotFoundException(string? message) : base(message)
        {
        }

        public IdNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
