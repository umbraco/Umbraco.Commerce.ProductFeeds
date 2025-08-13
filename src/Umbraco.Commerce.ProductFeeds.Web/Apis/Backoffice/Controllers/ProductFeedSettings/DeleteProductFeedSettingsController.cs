using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    public class DeleteProductFeedSettingsController : ManagementApiControllerBase
    {
        private readonly IProductFeedSettingsService _feedSettingsService;

        public DeleteProductFeedSettingsController(IProductFeedSettingsService feedSettingsService)
        {
            _feedSettingsService = feedSettingsService;
        }

        [Route("delete")]
        [HttpPost]
        [ProducesResponseType<bool>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete([FromForm] ICollection<Guid> ids)
        {
            IEnumerable<Task<bool>> deleteTasks = ids.Select(_feedSettingsService.DeleteSettingAsync);

            bool[] results = await Task.WhenAll(deleteTasks);
            bool success = results.All(success => success);
            if (!success)
            {
                return Problem("Some errors occurred, the data may not be deleted properly. Please check the server log.", statusCode: (int)HttpStatusCode.InternalServerError);
            }

            return Ok(success);
        }
    }
}
