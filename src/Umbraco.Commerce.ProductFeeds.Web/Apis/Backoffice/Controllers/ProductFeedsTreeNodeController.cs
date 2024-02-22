using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Commerce.ProductFeeds.Constants;

namespace Umbraco.Commerce.ProductFeeds.Web.Apis.Backoffice.Controllers
{
    [PluginController(RouteParams.AreaName)]
    public class ProductFeedsTreeNodeController : UmbracoAuthorizedApiController
    {
        [HttpGet]
        public IActionResult GetMenu()
        {
            // return an empty menu
            return Ok(Enumerable.Empty<object>());
        }
    }
}
