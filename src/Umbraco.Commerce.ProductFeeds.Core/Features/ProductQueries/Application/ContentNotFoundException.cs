using System.Text;

namespace Umbraco.Commerce.ProductFeeds.Core.Features.ProductQueries.Application
{
    [Serializable]
    public class ContentNotFoundException : Exception
    {
        private static readonly CompositeFormat _messageTemplate = CompositeFormat.Parse("Unable to find the content. {0}");


        public ContentNotFoundException()
        {
        }

        public ContentNotFoundException(string? message) : base(string.Format(null, _messageTemplate, message))
        {
        }

        public ContentNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
