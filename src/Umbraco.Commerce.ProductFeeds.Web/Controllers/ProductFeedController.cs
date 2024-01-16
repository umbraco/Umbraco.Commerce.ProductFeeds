using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Commerce.ProductFeeds.Constants;
using Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Application;
using Umbraco.Commerce.ProductFeeds.Core.FeedGenerators.Application;
using Umbraco.Commerce.ProductFeeds.Core.FeedSettings.Application;

namespace Umbraco.Commerce.ProductFeeds.Controllers
{
    [PluginController(RouteParams.AreaName)]
    public class ProductFeedController : UmbracoApiController
    {
        private readonly IProductFeedGeneratorFactory _productFeedService;
        private readonly IProductFeedSettingsService _feedConfigService;

        public ProductFeedController(
            IProductFeedGeneratorFactory productFeedService,
            IProductFeedSettingsService feedConfigService)
        {
            _productFeedService = productFeedService;
            _feedConfigService = feedConfigService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string path)
        {
            ProductFeedSettingReadModel? feedSettings = await _feedConfigService.FindSettingAsync(path).ConfigureAwait(true);
            if (feedSettings == null)
            {
                return NotFound("Unknown feed type.");
            }

            IProductFeedGeneratorService feedGenerator = _productFeedService.GetGenerator(feedSettings.FeedType);
            XmlDocument feed = feedGenerator.GenerateFeed(feedSettings);

            return Content(feed.OuterXml, "text/xml");
        }
    }
}
