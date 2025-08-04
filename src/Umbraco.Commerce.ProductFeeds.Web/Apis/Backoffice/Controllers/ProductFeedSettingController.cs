using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Asp.Versioning;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Management.Controllers;
using Umbraco.Cms.Core.Services;
using Umbraco.Commerce.Cms.Authorization;
using Umbraco.Commerce.ProductFeeds.Core.Common.Constants;
using Umbraco.Commerce.ProductFeeds.Core.Features.FeedGenerators.Implementations;
using Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Application;
using Umbraco.Commerce.ProductFeeds.Core.Features.PropertyValueExtractors.Implementations;
using Umbraco.Commerce.ProductFeeds.Web.Apis.Backoffice.Controllers.Models;

namespace Umbraco.Commerce.ProductFeeds.Web.Apis.Backoffice.Controllers
{
    [ApiVersion("1.0")]
    [MapToApi(RouteParams.ApiName)]
    [ProductFeedsVersionedApiBackofficeRoute("setting")]
    [ApiExplorerSettings(GroupName = "Settings")]
    [Authorize(UmbracoCommerceAuthorizationPolicies.SectionAccessCommerce)]
    public class ProductFeedSettingController : ManagementApiControllerBase
    {
        private readonly IProductFeedSettingsService _feedSettingsService;
        private readonly IContentTypeService _contentTypeService;
        private readonly SingleValuePropertyExtractorCollection _singleValuePropertyExtractors;
        private readonly MultipleValuePropertyExtractorCollection _multipleValuePropertyExtractors;
        private readonly FeedGeneratorCollection _feedGenerators;

        public ProductFeedSettingController(
            IProductFeedSettingsService feedConfigService,
            IContentTypeService contentTypeService,
            SingleValuePropertyExtractorCollection singleValuePropertyExtractors,
            MultipleValuePropertyExtractorCollection multipleValuePropertyExtractors,
            FeedGeneratorCollection feedGenerators)
        {
            _feedSettingsService = feedConfigService;
            _contentTypeService = contentTypeService;
            _singleValuePropertyExtractors = singleValuePropertyExtractors;
            _multipleValuePropertyExtractors = multipleValuePropertyExtractors;
            _feedGenerators = feedGenerators;
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
                return NotFound($"Couldn't find settings with id = '{id}'.");
            }

            return Ok(feedSetting);
        }

        [HttpGet("getbystore")]
        public async Task<ActionResult<List<ProductFeedSettingReadModel>>> GetByStore([FromQuery, BindRequired] Guid storeId)
        {
            List<ProductFeedSettingReadModel> feedSettings = await _feedSettingsService.GetListAsync(storeId).ConfigureAwait(true);
            return Ok(feedSettings);
        }

        [HttpGet("documenttypes")]
        public IActionResult GetDocumentTypes()
        {
            var aliases = _contentTypeService
                .GetAll()
                .Select(x => new
                {
                    x.Id,
                    x.Icon,
                    x.Name,
                    x.Description,
                    x.Key,
                    x.Alias,
                })
                .OrderBy(x => x.Name);

            return Ok(aliases);
        }

        [HttpGet("feedtypes")]
        public ActionResult<IEnumerable<LookupReadModel>> GetFeedTypes()
        {
            return Ok(_feedGenerators.Select(x => new LookupReadModel { Value = x.Id, Label = x.DisplayName }));
        }

        [Route("[action]")]
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

        [HttpGet("propertyvalueextractors")]
        public ActionResult<IEnumerable<LookupReadModel>> GetPropertyValueExtractors()
        {
            return Ok(_singleValuePropertyExtractors.Select(x => new LookupReadModel { Value = x.Id, Label = x.DisplayName })
                .Concat(_multipleValuePropertyExtractors.Select(x => new LookupReadModel { Value = x.Id, Label = x.DisplayName })));
        }
    }
}
