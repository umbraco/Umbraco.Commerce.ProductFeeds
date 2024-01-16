using System.Xml;
using Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Application;

namespace Umbraco.Commerce.ProductFeeds.Core.FeedGenerators.Application
{
    public interface IProductFeedGeneratorService
    {
        XmlDocument GenerateFeed(ProductFeedSettingReadModel feedSettings);
    }
}
