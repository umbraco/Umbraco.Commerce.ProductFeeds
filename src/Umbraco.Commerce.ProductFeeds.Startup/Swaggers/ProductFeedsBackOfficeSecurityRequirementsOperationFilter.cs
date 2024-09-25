using Umbraco.Cms.Api.Management.OpenApi;
using Umbraco.Commerce.ProductFeeds.Core.Common.Constants;

namespace Umbraco.Commerce.ProductFeeds.Startup.Swaggers
{
    public class ProductFeedsBackOfficeSecurityRequirementsOperationFilter : BackOfficeSecurityRequirementsOperationFilterBase
    {
        protected override string ApiName => RouteParams.ApiName;
    }
}
