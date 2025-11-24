using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using Umbraco.Commerce.ProductFeeds.Core.Common.Constants;

namespace Umbraco.Commerce.ProductFeeds.Startup.Swaggers
{
    public class ProductFeedsConfigureSwaggerGenOptions : IConfigureOptions<SwaggerGenOptions>
    {
        public void Configure(SwaggerGenOptions options)
        {
            options.SwaggerDoc(RouteParams.ApiName, new OpenApiInfo
            {
                Title = RouteParams.ApiTitle,
                Version = "Latest",
                Description = "Describes Umbraco Commerce Product Feeds package management APIs",
            });

            options.OperationFilter<ProductFeedsBackOfficeSecurityRequirementsOperationFilter>();
            options.OperationFilter<AddApiVersionToOperationIdFilter>();
        }
    }
}
