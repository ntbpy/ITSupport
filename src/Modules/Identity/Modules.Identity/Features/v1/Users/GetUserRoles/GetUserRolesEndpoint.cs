using MIT.Modules.Identity.Contracts.Authorization;
using MIT.Framework.Shared.Identity.Authorization;
using MIT.Modules.Identity.Contracts.DTOs;
using MIT.Modules.Identity.Contracts.v1.Users.GetUserRoles;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace MIT.Modules.Identity.Features.v1.Users.GetUserRoles;

public static class GetUserRolesEndpoint
{
    internal static RouteHandlerBuilder MapGetUserRolesEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/users/{id:guid}/roles", async (string id, IMediator mediator, CancellationToken cancellationToken) =>
            TypedResults.Ok(await mediator.Send(new GetUserRolesQuery(id), cancellationToken)))
        .WithName("GetUserRoles")
        .WithSummary("Get user roles")
        .RequirePermission(IdentityPermissions.Users.View)
        .WithDescription("Retrieve the roles assigned to a specific user.")
        .Produces<IEnumerable<UserRoleDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);
    }
}