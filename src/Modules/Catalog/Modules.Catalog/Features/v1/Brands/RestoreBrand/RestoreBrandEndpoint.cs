using MIT.Framework.Shared.Identity.Authorization;
using MIT.Framework.Web.Idempotency;
using MIT.Modules.Catalog.Contracts.Authorization;
using MIT.Modules.Catalog.Contracts.v1.Brands;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace MIT.Modules.Catalog.Features.v1.Brands.RestoreBrand;

public static class RestoreBrandEndpoint
{
    internal static RouteHandlerBuilder MapRestoreBrandEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/brands/{brandId:guid}/restore",
                async (Guid brandId, IMediator mediator, CancellationToken ct) =>
                    Results.Ok(await mediator.Send(new RestoreBrandCommand(brandId), ct)))
            .WithName("RestoreBrand")
            .WithSummary("Restore a soft-deleted brand")
            .RequirePermission(CatalogPermissions.Brands.Restore)
            .WithIdempotency();
    }
}
