using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.BackOffice.Filters;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Commerce.ProductFeeds.Core.Common.Constants;
using Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Application;
using Umbraco.Commerce.ProductFeeds.Core.FeedSettings.Application;

namespace Umbraco.Commerce.ProductFeeds.Controllers
{
    [JsonCamelCaseFormatter]
    [PluginController(RouteParams.AreaName)]
    public class ProductFeedSettingListController : UmbracoAuthorizedApiController
    {
        private readonly IProductFeedSettingsService _feedSettingsService;

        public ProductFeedSettingListController(IProductFeedSettingsService feedConfigService)
        {
            _feedSettingsService = feedConfigService;
        }

        public async Task<IActionResult> Get(Guid storeId)
        {
            List<ProductFeedSettingReadModel> feedSettings = await _feedSettingsService.GetListAsync(storeId).ConfigureAwait(true);
            return Ok(feedSettings);
        }
    }
}
