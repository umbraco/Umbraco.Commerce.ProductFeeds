using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Commerce.ProductFeeds.Constants;
using Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Application;
using Umbraco.Commerce.ProductFeeds.Core.FeedSettings.Application;

namespace Umbraco.Commerce.ProductFeeds.Controllers
{
    [PluginController(RouteParams.AreaName)]
    public class ProductFeedSettingController : UmbracoAuthorizedApiController
    {
        private readonly IProductFeedSettingsService _feedSettingsService;

        public ProductFeedSettingController(IProductFeedSettingsService feedConfigService)
        {
            _feedSettingsService = feedConfigService;
        }

        [HttpPost]
        public async Task<IActionResult> Save(ProductFeedSettingAddModel model)
        {
            int? recordId = await _feedSettingsService.SaveSettingAsync(model).ConfigureAwait(true);
            if (recordId == null)
            {
                return Problem("Save failed", statusCode: (int)HttpStatusCode.InternalServerError);
            }

            CreatedResult result = Created();
            result.Value = recordId;
            return result;
        }

        public async Task<IActionResult> Get(int id)
        {
            ProductFeedSettingReadModel? feedSetting = await _feedSettingsService
                .FindSettingAsync(new FindSettingParams { Id = id })
                .ConfigureAwait(true);
            if (feedSetting == null)
            {
                return NotFound($"Couldn't find settings with path = '{id}'.");
            }

            return new JsonResult(feedSetting);
        }
    }
}
