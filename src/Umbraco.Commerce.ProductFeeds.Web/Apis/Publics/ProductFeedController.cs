using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Commerce.ProductFeeds.Core.Features.FeedGenerators.Application;
using Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Application;

namespace Umbraco.Commerce.ProductFeeds.Web.Apis.Publics
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

        public async Task<IActionResult> Generate(string path)
        {
            ProductFeedSettingReadModel? feedSettings = await _feedConfigService
                .FindSettingAsync(new FindSettingParams { FeedRelativePath = path })
                .ConfigureAwait(true);
            if (feedSettings == null)
            {
                return NotFound("Unknown feed type.");
            }

            IProductFeedGeneratorService feedGenerator = _feedGeneratorFactory.GetGenerator(feedSettings.FeedGeneratorId);

            switch (feedGenerator.Format)
            {
                case FeedFormat.Xml:
                    XmlDocument xmlFeed = await feedGenerator.GenerateXmlFeedAsync(feedSettings);
                    var result = new XmlActionResult(xmlFeed) { Formatting = Formatting.Indented };
                    return result;
                case FeedFormat.Json:
                    JsonDocument jsonFeed = await feedGenerator.GenerateJsonFeedAsync(feedSettings);
                    var jsonResult = new JsonResult(jsonFeed.RootElement);
                    return jsonResult;
                default:
                    return Problem("Unknown feed format.");
            }
        }
    }
}
