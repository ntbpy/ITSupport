using MIT.Framework.Shared.Identity.Authorization;
using MIT.Modules.Chat.Contracts.Authorization;
using MIT.Modules.Chat.Contracts.v1.Commands;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace MIT.Modules.Chat.Features.v1.Messages.PinMessage;

public static class PinMessageEndpoint
{
    internal static RouteHandlerBuilder MapPinMessageEndpoint(this IEndpointRouteBuilder endpoints)
        => endpoints.MapPost("/messages/{id:guid}/pin",
                async (Guid id, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    await mediator.Send(new PinMessageCommand(id), cancellationToken);
                    return Results.NoContent();
                })
            .WithName("PinMessage")
            .WithSummary("Pin a message to its channel")
            .RequirePermission(ChatPermissions.Messages.Send);
}
