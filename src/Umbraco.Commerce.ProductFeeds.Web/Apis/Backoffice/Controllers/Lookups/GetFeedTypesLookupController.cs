using System.Collections.Generic;
using System.Linq;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Management.Controllers;
using Umbraco.Commerce.Cms.Authorization;
using Umbraco.Commerce.ProductFeeds.Core.Common.Constants;
using Umbraco.Commerce.ProductFeeds.Core.Features.FeedGenerators.Implementations;
using Umbraco.Commerce.ProductFeeds.Web.Apis.Backoffice.Controllers.Models;

namespace Umbraco.Commerce.ProductFeeds.Web.Apis.Backoffice.Controllers.Lookups
{
    [ApiVersion("2.0")]
    [MapToApi(RouteParams.ApiName)]
    [ProductFeedsVersionedApiBackofficeRoute("setting")]
    [ApiExplorerSettings(GroupName = "Settings")]
    [Authorize(UmbracoCommerceAuthorizationPolicies.SectionAccessCommerce)]
    public class GetFeedTypesLookupController : ManagementApiControllerBase
    {
        private readonly FeedGeneratorCollection _feedGenerators;

        public GetFeedTypesLookupController(FeedGeneratorCollection feedGenerators)
        {
            _feedGenerators = feedGenerators;
        }

        [HttpGet("feedgenerators")]
        public ActionResult<IEnumerable<LookupReadModel>> GetFeedGenerators()
        {
            return Ok(_feedGenerators.Select(x => new LookupReadModel { Value = x.Id.ToString(), Label = x.DisplayName }));
        }
    }
}
