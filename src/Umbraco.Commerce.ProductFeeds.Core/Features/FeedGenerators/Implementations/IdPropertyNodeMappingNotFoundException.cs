namespace Umbraco.Commerce.ProductFeeds.Core.Features.FeedGenerators.Implementations
{
    public class IdPropertyNodeMappingNotFoundException : Exception
    {
        public IdPropertyNodeMappingNotFoundException() : base("id property mapping not found") { }

        public IdPropertyNodeMappingNotFoundException(string message) : base(message)
        {
        }

        public IdPropertyNodeMappingNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
