using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Commerce.ProductFeeds.Constants;
using Umbraco.Commerce.ProductFeeds.Core.FeedSettings.Application;

namespace Umbraco.Commerce.ProductFeeds.Controllers
{
    [PluginController(RouteParams.AreaName)]
    public class ProductFeedSettingController : UmbracoAuthorizedApiController
    {
        private readonly IProductFeedSettingsService _feedConfigService;

        public ProductFeedSettingController(IProductFeedSettingsService feedConfigService)
        {
            _feedConfigService = feedConfigService;
        }

        [HttpPost]
        public async Task<IActionResult> Save(ProductFeedSettingAddModel model)
        {
            int? recordId = await _feedConfigService.SaveSettingAsync(model).ConfigureAwait(true);
            if (recordId == null)
            {
                return Problem("Save failed", statusCode: (int)HttpStatusCode.InternalServerError);
            }

            CreatedResult result = Created();
            result.Value = recordId;
            return result;
        }
    }
}
