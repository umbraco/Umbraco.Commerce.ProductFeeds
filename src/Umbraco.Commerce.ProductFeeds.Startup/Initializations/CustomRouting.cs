using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Web.Common.Routing;
using Umbraco.Commerce.ProductFeeds.Core.Constants;

namespace Umbraco.Commerce.ProductFeeds.Startup.Initializations
{
    public class CustomRouting : IAreaRoutes
    {
        private readonly GlobalSettings _globalSettings;
        private readonly IHostingEnvironment _hostingEnvironment;

        public CustomRouting(IOptions<GlobalSettings> globalSettings, IHostingEnvironment hostingEnvironment)
        {
            _globalSettings = globalSettings?.Value ?? throw new ArgumentNullException(nameof(globalSettings));
            _hostingEnvironment = hostingEnvironment;
        }

        public void CreateRoutes(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllerRoute(
                RouteParams.AreaName,
                _globalSettings.GetUmbracoMvcArea(_hostingEnvironment) + "/commerce/productfeed/{action}/{path}",
                new
                {
                    //controller = ControllerExtensions.GetControllerName(typeof(ProductFeedController)),
                    controller = "ProductFeed",
                    Action = "Xml",
                });
        }
    }
}
