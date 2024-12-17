using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Application;
using Umbraco.Commerce.ProductFeeds.Core.FeedGenerators.Application;
using Umbraco.Commerce.ProductFeeds.Core.FeedSettings.Application;

namespace Umbraco.Commerce.ProductFeeds.Apis.Public
{
    public class ProductFeedController : ControllerBase
    {
        private readonly IProductFeedGeneratorFactory _feedGeneratorFactory;
        private readonly IProductFeedSettingsService _feedConfigService;

        public ProductFeedController(
            IProductFeedGeneratorFactory productFeedService,
            IProductFeedSettingsService feedConfigService)
        {
            _feedGeneratorFactory = productFeedService;
            _feedConfigService = feedConfigService;
        }

        public async Task<IActionResult> Xml(string path)
        {
            ProductFeedSettingReadModel? feedSettings = await _feedConfigService
                .FindSettingAsync(new FindSettingParams {FeedRelativePath = path})
                .ConfigureAwait(true);
            if (feedSettings == null)
            {
                return NotFound("Unknown feed type.");
            }

            IProductFeedGeneratorService feedGenerator = _feedGeneratorFactory.GetGenerator(feedSettings.FeedType);
            XmlDocument feed = feedGenerator.GenerateFeed(feedSettings);

            var result = new XmlActionResult(feed) {Formatting = Formatting.Indented};
            return result;
        }
    }
}
