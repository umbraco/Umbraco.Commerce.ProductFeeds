using Asp.Versioning;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Umbraco.Commerce.ProductFeeds.Startup.Swaggers
{
    internal class AddApiVersionToOperationIdTransformer : IOpenApiOperationTransformer
    {
        public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
        {
            // Swashbuckle's OperationFilterContext.ApiDescription.GetApiVersion() is no longer available
            // under Microsoft.AspNetCore.OpenApi, so read the declared version from the action's
            // ApiVersionMetadata instead (typically "v1", "v2", etc.).
            ApiVersionMetadata? metadata = context.Description.ActionDescriptor.EndpointMetadata
                .OfType<ApiVersionMetadata>()
                .FirstOrDefault();

            ApiVersion? version = metadata?
                .Map(ApiVersionMapping.Explicit | ApiVersionMapping.Implicit)
                .DeclaredApiVersions
                .FirstOrDefault();

            if (version != null && version.MajorVersion > 1)
            {
                operation.OperationId += $"_v{version.MajorVersion ?? 1}";
            }

            return Task.CompletedTask;
        }
    }
}
