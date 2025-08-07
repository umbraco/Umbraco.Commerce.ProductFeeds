using System.Collections.Generic;
using System.Linq;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Management.Controllers;
using Umbraco.Commerce.Cms.Authorization;
using Umbraco.Commerce.ProductFeeds.Core.Common.Constants;
using Umbraco.Commerce.ProductFeeds.Core.Features.PropertyValueExtractors.Implementations;
using Umbraco.Commerce.ProductFeeds.Web.Apis.Backoffice.Controllers.Models;

namespace Umbraco.Commerce.ProductFeeds.Web.Apis.Backoffice.Controllers.Lookups
{
    [ApiVersion("2.0")]
    [MapToApi(RouteParams.ApiName)]
    [ProductFeedsVersionedApiBackofficeRoute("setting")]
    [ApiExplorerSettings(GroupName = "Settings")]
    [Authorize(UmbracoCommerceAuthorizationPolicies.SectionAccessCommerce)]
    public class GetPropertyValueExtractorsLookupController : ManagementApiControllerBase
    {
        private readonly SingleValuePropertyExtractorCollection _singleValuePropertyExtractors;
        private readonly MultipleValuePropertyExtractorCollection _multipleValuePropertyExtractors;

        public GetPropertyValueExtractorsLookupController(
            SingleValuePropertyExtractorCollection singleValuePropertyExtractors,
            MultipleValuePropertyExtractorCollection multipleValuePropertyExtractors)
        {
            _singleValuePropertyExtractors = singleValuePropertyExtractors;
            _multipleValuePropertyExtractors = multipleValuePropertyExtractors;
        }

        [HttpGet("propertyvalueextractors")]
        public ActionResult<IEnumerable<LookupReadModel>> GetPropertyValueExtractors()
        {
            return Ok(_singleValuePropertyExtractors.Select(x => new LookupReadModel { Value = x.Id, Label = x.DisplayName })
                .Concat(_multipleValuePropertyExtractors.Select(x => new LookupReadModel { Value = x.Id, Label = x.DisplayName })));
        }
    }
}
