using System;
using System.Net;
using System.Threading.Tasks;
using Asp.Versioning;
using FluentValidation.Results;
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
    public class SaveProductFeedSettingController : ManagementApiControllerBase
    {
        private readonly IProductFeedSettingsService _feedSettingsService;

        public SaveProductFeedSettingController(IProductFeedSettingsService feedSettingsService)
        {
            _feedSettingsService = feedSettingsService;
        }

        [HttpPost("save")]
        [ProducesResponseType<Guid>(StatusCodes.Status200OK)]
        [ProducesResponseType<Guid>(StatusCodes.Status201Created)]
        [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Save(ProductFeedSettingWriteModel? model)
        {
            if (model == null)
            {
                return BadRequest("Unable to bind posted data to model.");
            }

            var validator = new ProductFeedSettingWriteModelValidator();
            ValidationResult validationResult = await validator.ValidateAsync(model);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            Guid? recordId = await _feedSettingsService.SaveSettingAsync(model);
            if (recordId == null)
            {
                return Problem("Save failed.", statusCode: (int)HttpStatusCode.InternalServerError);
            }

            ContentResult actionResult = new()
            {
                Content = recordId.ToString(),
                StatusCode = model.Id.HasValue ? (int)HttpStatusCode.OK : (int)HttpStatusCode.Created,
            };

            return actionResult;
        }
    }
}
