using Asp.Versioning;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Umbraco.Commerce.ProductFeeds.Startup.Swaggers
{
    /// <summary>
    /// Reproduces the v17 operation id scheme so the generated API client keeps the same method names
    /// after the Umbraco 18 move from Swashbuckle to Microsoft.AspNetCore.OpenApi.
    /// <para>
    /// The .NET OpenAPI pipeline that replaced Swashbuckle derives operation ids from the HTTP verb and
    /// route (e.g. "getSettingGetbystore"), which would rename every generated client method. Following
    /// umbraco/Umbraco.Commerce PR #151 (CommerceOpenApiOperationTransformer), set the id to the controller
    /// action name with the first character lower-cased instead. The API major version is then appended for
    /// v2+ (the behaviour of the former AddApiVersionToOperationIdFilter), e.g.
    /// "GetByStore" -> "getByStore_v2" -> (openapi-ts) "getByStoreV2".
    /// </para>
    /// </summary>
    internal sealed class ProductFeedsOperationIdTransformer : IOpenApiOperationTransformer
    {
        public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
        {
            if (context.Description.ActionDescriptor is not ControllerActionDescriptor descriptor)
            {
                return Task.CompletedTask;
            }

            var action = descriptor.RouteValues.TryGetValue("action", out var actionName) && !string.IsNullOrEmpty(actionName)
                ? actionName
                : descriptor.ActionName;

            if (string.IsNullOrEmpty(action))
            {
                return Task.CompletedTask;
            }

            var operationId = char.ToLowerInvariant(action[0]) + action[1..];

            ApiVersion? version = context.Description.ActionDescriptor.EndpointMetadata
                .OfType<ApiVersionMetadata>()
                .FirstOrDefault()?
                .Map(ApiVersionMapping.Explicit | ApiVersionMapping.Implicit)
                .DeclaredApiVersions
                .FirstOrDefault();

            if (version != null && version.MajorVersion > 1)
            {
                operationId += $"_v{version.MajorVersion ?? 1}";
            }

            operation.OperationId = operationId;

            return Task.CompletedTask;
        }
    }
}
