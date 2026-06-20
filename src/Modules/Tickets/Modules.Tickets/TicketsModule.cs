using Asp.Versioning;
using MIT.Framework.Persistence;
using MIT.Framework.Shared.Constants;
using MIT.Framework.Web.Modules;
using MIT.Modules.Tickets.Contracts.Authorization;
using MIT.Modules.Tickets.Data;
using MIT.Modules.Tickets.Features.v1.Tickets.AddTicketComment;
using MIT.Modules.Tickets.Features.v1.Tickets.AssignTicket;
using MIT.Modules.Tickets.Features.v1.Tickets.CloseTicket;
using MIT.Modules.Tickets.Features.v1.Tickets.CreateTicket;
using MIT.Modules.Tickets.Features.v1.Tickets.DeleteTicket;
using MIT.Modules.Tickets.Features.v1.Tickets.GetTicketById;
using MIT.Modules.Tickets.Features.v1.Tickets.ListTicketComments;
using MIT.Modules.Tickets.Features.v1.Tickets.ListTrashedTickets;
using MIT.Modules.Tickets.Features.v1.Tickets.ReopenTicket;
using MIT.Modules.Tickets.Features.v1.Tickets.ResolveTicket;
using MIT.Modules.Tickets.Features.v1.Tickets.RestoreTicket;
using MIT.Modules.Tickets.Features.v1.Tickets.SearchTickets;
using MIT.Modules.Tickets.Features.v1.Tickets.UpdateTicket;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

[assembly: FshModule(typeof(MIT.Modules.Tickets.TicketsModule), 700)]

namespace MIT.Modules.Tickets;

public sealed class TicketsModule : IModule
{
    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        PermissionConstants.Register(TicketsPermissions.All);

        builder.Services.AddHeroDbContext<TicketsDbContext>();
        builder.Services.AddScoped<IDbInitializer, TicketsDbInitializer>();

        builder.Services.AddHealthChecks()
            .AddDbContextCheck<TicketsDbContext>(
                name: "db:tickets",
                failureStatus: HealthStatus.Unhealthy);
    }

    public void ConfigureMiddleware(IApplicationBuilder app)
    {
        // No custom middleware needed
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        var versionSet = endpoints.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();

        var group = endpoints
            .MapGroup("api/v{version:apiVersion}")
            .WithTags("Tickets")
            .WithApiVersionSet(versionSet)
            .RequireAuthorization();

        // Trash + comment routes register before the catch-all `{ticketId:guid}` GET so literal
        // segments win — minimal APIs match the first compatible pattern, so order matters.
        group.MapListTrashedTicketsEndpoint();
        group.MapAddTicketCommentEndpoint();
        group.MapListTicketCommentsEndpoint();

        group.MapRestoreTicketEndpoint();
        group.MapAssignTicketEndpoint();
        group.MapResolveTicketEndpoint();
        group.MapReopenTicketEndpoint();
        group.MapCloseTicketEndpoint();

        group.MapCreateTicketEndpoint();
        group.MapSearchTicketsEndpoint();
        group.MapUpdateTicketEndpoint();
        group.MapDeleteTicketEndpoint();
        group.MapGetTicketByIdEndpoint();
    }
}
