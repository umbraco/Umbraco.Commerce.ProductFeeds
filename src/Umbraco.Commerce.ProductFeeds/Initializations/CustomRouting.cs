using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Web.Common.Routing;
using Umbraco.Commerce.ProductFeeds.Apis.Public;
using Umbraco.Commerce.ProductFeeds.Constants;
using Umbraco.Extensions;

namespace Umbraco.Commerce.ProductFeeds.Initializations
{
    public class CustomRouting : IAreaRoutes
    {
        private readonly GlobalSettings _globalSettings;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly string _umbracoPathSegment;

        public CustomRouting(IOptions<GlobalSettings> globalSettings, IHostingEnvironment hostingEnvironment)
        {
            _globalSettings = globalSettings?.Value ?? throw new ArgumentNullException(nameof(globalSettings));
            _hostingEnvironment = hostingEnvironment;
            _umbracoPathSegment = _globalSettings.GetUmbracoMvcArea(_hostingEnvironment);

        }

        public void CreateRoutes(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllerRoute(
                RouteParams.AreaName,
                _globalSettings.GetUmbracoMvcArea(_hostingEnvironment) + "/commerce/productfeed/{action}/{path}",
                new
                {
                    controller = ControllerExtensions.GetControllerName(typeof(ProductFeedController)),
                    Action = "Xml",
                });
        }
    }
}
