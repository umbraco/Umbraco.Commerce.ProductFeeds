using System;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class GetProductFeedSettingByIdController : ManagementApiControllerBase
    {
        private readonly IProductFeedSettingsService _feedSettingsService;

        public GetProductFeedSettingByIdController(IProductFeedSettingsService feedSettingsService)
        {
            _feedSettingsService = feedSettingsService;
        }

        [HttpGet("get/{id}")]
        [ProducesResponseType<ProductFeedSettingReadModel>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDetails(Guid id)
        {
            ProductFeedSettingReadModel? feedSetting = await _feedSettingsService
                .FindSettingAsync(new FindSettingParams { Id = id })
                .ConfigureAwait(true);
            if (feedSetting == null)
            {
                return NotFound($"Couldn't find the feed setting with id = '{id}'.");
            }

            return Ok(feedSetting);
        }
    }
}
