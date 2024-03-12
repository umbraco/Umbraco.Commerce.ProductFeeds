using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.BackOffice.Filters;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Commerce.ProductFeeds.Constants;
using Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Application;
using Umbraco.Commerce.ProductFeeds.Core.FeedSettings.Application;
using Umbraco.Commerce.ProductFeeds.Core.PropertyValueExtractors.Implementations;

namespace Umbraco.Commerce.ProductFeeds.Controllers
{
    [JsonCamelCaseFormatter]
    [PluginController(RouteParams.AreaName)]
    public class ProductFeedSettingController : UmbracoAuthorizedApiController
    {
        private readonly IProductFeedSettingsService _feedSettingsService;
        private readonly IContentTypeService _contentTypeService;
        private readonly PropertyExtractorNameTypeMapping _propertyExtractorNameTypeMapping;

        public ProductFeedSettingController(
            IProductFeedSettingsService feedConfigService,
            IContentTypeService contentTypeService,
            PropertyExtractorNameTypeMapping propertyExtractorNameTypeMapping)
        {
            _feedSettingsService = feedConfigService;
            _contentTypeService = contentTypeService;
            _propertyExtractorNameTypeMapping = propertyExtractorNameTypeMapping;
        }

        [HttpPost]
        public async Task<IActionResult> Save(ProductFeedSettingWriteModel? model)
        {
            if (model == null)
            {
                return BadRequest("Unable to bind posted data to model.");
            }

            ProductFeedSettingWriteModelValidator validator = new ProductFeedSettingWriteModelValidator();
            ValidationResult validationResult = await validator.ValidateAsync(model).ConfigureAwait(true);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            Guid? recordId = await _feedSettingsService.SaveSettingAsync(model).ConfigureAwait(true);
            if (recordId == null)
            {
                return Problem("Save failed.", statusCode: (int)HttpStatusCode.InternalServerError);
            }

            CreatedResult result = Created();
            result.Value = recordId;
            return result;
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid id)
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

        [HttpGet]
        public IActionResult GetDocumentTypes()
        {
            IEnumerable<string> aliases = _contentTypeService.GetAllContentTypeAliases()
                .OrderBy(x => x);

            return Ok(aliases);
        }

        /// <summary>
        /// Get all property aliases of requested document types.
        /// </summary>
        /// <param name="documentTypeAliases">Multiple aliases that are separated by ';' character.</param>
        /// <returns>An ordered string[].</returns>
        [HttpGet]
        public IActionResult GetPropertyAliases(string documentTypeAliases)
        {
            if (string.IsNullOrWhiteSpace(documentTypeAliases))
            {
                return BadRequest($"{nameof(documentTypeAliases)} must not be empty.");
            }

            string[] docTypeAliases = documentTypeAliases.Split(';');
            HashSet<string> propertyAliases = new();
            foreach (string doctTypeAlias in docTypeAliases)
            {
                IContentType? documentType = _contentTypeService.Get(doctTypeAlias);
                if (documentType == null)
                {
                    continue;
                }

                propertyAliases.UnionWith(documentType.PropertyTypes.Select(x => x.Alias).OrderBy(alias => alias));
            }

            return Ok(propertyAliases.Order());
        }

        [HttpGet]
        public IActionResult GetFeedTypes()
        {
            return Ok(new string[] { ProductFeedType.GoogleMerchantCenter.ToString() });
        }

        [HttpPost]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] Guid id)
        {
            bool success = await _feedSettingsService.DeleteSettingAsync(id).ConfigureAwait(true);
            if (!success)
            {
                return Problem("Delete failed.", statusCode: (int)HttpStatusCode.InternalServerError);
            }

            return Ok(success);
        }

        [HttpGet]
        public IActionResult GetPropertyValueExtractors()
        {
            return Ok(_propertyExtractorNameTypeMapping.ReadOnlyDictionary.Keys);
        }
    }
}
