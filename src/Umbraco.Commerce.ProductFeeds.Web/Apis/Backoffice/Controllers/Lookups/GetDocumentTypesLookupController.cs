using System.Linq;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Management.Controllers;
using Umbraco.Cms.Core.Services;
using Umbraco.Commerce.Cms.Authorization;
using Umbraco.Commerce.ProductFeeds.Core.Common.Constants;

namespace Umbraco.Commerce.ProductFeeds.Web.Apis.Backoffice.Controllers.Lookups
{
    [ApiVersion("2.0")]
    [MapToApi(RouteParams.ApiName)]
    [ProductFeedsVersionedApiBackofficeRoute("setting")]
    [ApiExplorerSettings(GroupName = "Settings")]
    [Authorize(UmbracoCommerceAuthorizationPolicies.SectionAccessCommerce)]
    public class GetDocumentTypesLookupController : ManagementApiControllerBase
    {
        private readonly IContentTypeService _contentTypeService;

        public GetDocumentTypesLookupController(IContentTypeService contentTypeService)
        {
            _contentTypeService = contentTypeService;
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
    }
}
