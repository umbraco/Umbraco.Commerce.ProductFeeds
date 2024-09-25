using Umbraco.Cms.Web.Common.Routing;
using Umbraco.Commerce.ProductFeeds.Core.Common.Constants;

namespace Umbraco.Commerce.ProductFeeds.Web.Apis.Backoffice
{
    internal sealed class ProductFeedsVersionedApiBackofficeRoute : BackOfficeRouteAttribute
    {
        public ProductFeedsVersionedApiBackofficeRoute(string template)
            : base($"{RouteParams.ApiName}/management/api/v{{version:apiVersion}}/{template.TrimStart('/')}")
        {
        }
    }
}
