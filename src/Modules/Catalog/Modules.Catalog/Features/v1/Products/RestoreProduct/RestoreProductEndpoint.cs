using MIT.Framework.Shared.Identity.Authorization;
using MIT.Framework.Web.Idempotency;
using MIT.Modules.Catalog.Contracts.Authorization;
using MIT.Modules.Catalog.Contracts.v1.Products;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace MIT.Modules.Catalog.Features.v1.Products.RestoreProduct;

public static class RestoreProductEndpoint
{
    internal static RouteHandlerBuilder MapRestoreProductEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/products/{productId:guid}/restore",
                async (Guid productId, IMediator mediator, CancellationToken ct) =>
                    Results.Ok(await mediator.Send(new RestoreProductCommand(productId), ct)))
            .WithName("RestoreProduct")
            .WithSummary("Restore a soft-deleted product")
            .RequirePermission(CatalogPermissions.Products.Restore)
            .WithIdempotency();
    }
}
