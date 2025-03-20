using System.Xml;
using Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Application;

namespace Umbraco.Commerce.ProductFeeds.Core.Features.FeedGenerators.Application
{
    public interface IProductFeedGeneratorService
    {
        Task<XmlDocument> GenerateFeedAsync(ProductFeedSettingReadModel feedSetting);
    }
}
