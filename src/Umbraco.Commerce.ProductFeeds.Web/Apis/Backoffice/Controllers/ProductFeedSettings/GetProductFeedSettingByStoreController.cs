using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Management.Controllers;
using Umbraco.Commerce.Cms.Authorization;
using Umbraco.Commerce.ProductFeeds.Core.Common.Constants;
using Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Application;

namespace Umbraco.Commerce.ProductFeeds.Web.Apis.Backoffice.Controllers.ProductFeedSettings
{
    [ApiVersion("2.0")]
    [MapToApi(RouteParams.ApiName)]
    [ProductFeedsVersionedApiBackofficeRoute("setting")]
    [ApiExplorerSettings(GroupName = "Settings")]
    [Authorize(UmbracoCommerceAuthorizationPolicies.SectionAccessCommerce)]
    public class GetProductFeedSettingByStoreController : ManagementApiControllerBase
    {
        private readonly IProductFeedSettingsService _feedSettingsService;

        public GetProductFeedSettingByStoreController(IProductFeedSettingsService feedSettingsService)
        {
            _feedSettingsService = feedSettingsService;
        }

        [HttpGet("getbystore")]
        public async Task<ActionResult<List<ProductFeedSettingReadModel>>> GetByStore([FromQuery, BindRequired] Guid storeId)
        {
            List<ProductFeedSettingReadModel> feedSettings = await _feedSettingsService.GetListAsync(storeId).ConfigureAwait(true);
            return Ok(feedSettings);
        }
    }
}
