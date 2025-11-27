using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Umbraco.Commerce.ProductFeeds.Startup.Swaggers
{
    internal class AddApiVersionToOperationIdFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            Asp.Versioning.ApiVersion? version = context.ApiDescription.GetApiVersion(); // typically "v1", "v2", etc.

            if (version != null && version.MajorVersion > 1)
            {
                operation.OperationId += $"_v{version.MajorVersion ?? 1}";
            }
        }
    }
}
