using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Umbraco.Cms.Web.Common.Routing;
using Umbraco.Commerce.ProductFeeds.Core.Common.Constants;

namespace Umbraco.Commerce.ProductFeeds.Startup.Initializations
{
    public class CustomRouting : IAreaRoutes
    {
        public void CreateRoutes(IEndpointRouteBuilder endpoints) => endpoints.MapControllerRoute(
            RouteParams.AreaName,
            Umbraco.Cms.Core.Constants.System.UmbracoPathSegment + "/commerce/productfeed/{path}/{action}",
            new
            {
                controller = "ProductFeed",
                Action = "Generate",
            });
    }
}
