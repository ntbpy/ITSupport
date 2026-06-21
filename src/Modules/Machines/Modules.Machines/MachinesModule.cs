using Asp.Versioning;
using MIT.Framework.Persistence;
using MIT.Framework.Shared.Constants;
using MIT.Framework.Shared.Identity.Authorization;
using MIT.Framework.Web.Modules;
using MIT.Modules.Machines.Contracts.Authorization;
using MIT.Modules.Machines.Contracts.v1.Machines;
using MIT.Modules.Machines.Data;
using MIT.Modules.Machines.Infrastructure;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

[assembly: FshModule(typeof(MIT.Modules.Machines.MachinesModule), 800)]

namespace MIT.Modules.Machines;

public sealed class MachinesModule : IModule
{
    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        PermissionConstants.Register(MachinesPermissions.All);

        builder.Services.AddHeroDbContext<MachinesDbContext>();
        builder.Services.AddScoped<IDbInitializer, MachinesDbInitializer>();
        builder.Services.AddHealthChecks()
            .AddDbContextCheck<MachinesDbContext>(
                name: "db:machines",
                failureStatus: HealthStatus.Unhealthy);

        builder.Services.Configure<AgentKeyOptions>(
            builder.Configuration.GetSection(AgentKeyOptions.Section));
        builder.Services.AddScoped<IAgentApiKeyService, AgentApiKeyService>();
        builder.Services.AddScoped<IAgentMetricsPublisher, AgentMetricsPublisher>();
        builder.Services.AddScoped<IMachineCommandQueueService, MachineCommandQueueService>();
    }

    public void ConfigureMiddleware(IApplicationBuilder app)
    {
        app.UseMiddleware<AgentAuthMiddleware>();
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
            .WithTags("Machines")
            .WithApiVersionSet(versionSet);

        // Agent endpoints — authenticated via AgentAuthMiddleware (X-Agent-Key header), not JWT.
        var agentGroup = group.MapGroup("agent");

        agentGroup.MapPost("/register",
            async (RegisterAgentCommand cmd, IMediator mediator, CancellationToken ct) =>
                Results.Ok(await mediator.Send(cmd, ct)))
            .WithName("AgentRegister")
            .AllowAnonymous();

        agentGroup.MapPost("/heartbeat",
            async (HeartbeatCommand cmd, IMediator mediator, CancellationToken ct) =>
                Results.Ok(await mediator.Send(cmd, ct)))
            .WithName("AgentHeartbeat")
            .AllowAnonymous();

        agentGroup.MapPost("/snapshot",
            async (PostSnapshotCommand cmd, IMediator mediator, CancellationToken ct) =>
            {
                await mediator.Send(cmd, ct);
                return Results.Ok();
            })
            .WithName("AgentPostSnapshot")
            .AllowAnonymous();

        agentGroup.MapGet("/commands",
            async (HttpContext ctx, IMediator mediator, CancellationToken ct) =>
            {
                var machineId = (Guid)ctx.Items["MachineId"]!;
                return Results.Ok(await mediator.Send(new GetCommandsQuery(machineId), ct));
            })
            .WithName("AgentGetCommands")
            .AllowAnonymous();

        agentGroup.MapPost("/commands/{commandId:guid}/result",
            async (Guid commandId, PostCommandResultRequest req, HttpContext ctx,
                IMediator mediator, CancellationToken ct) =>
            {
                var machineId = (Guid)ctx.Items["MachineId"]!;
                await mediator.Send(new PostCommandResultCommand(
                    machineId, commandId, req.Success, req.OutputJson, req.ErrorMessage), ct);
                return Results.Ok();
            })
            .WithName("AgentPostCommandResult")
            .AllowAnonymous();

        // Admin endpoints — JWT required.
        var adminGroup = group.MapGroup("admin").RequireAuthorization();

        adminGroup.MapGet("/machines",
            async ([AsParameters] ListMachinesQuery q, IMediator m, CancellationToken ct) =>
                Results.Ok(await m.Send(q, ct)))
            .WithName("ListMachines")
            .RequirePermission(MachinesPermissions.Machines.View);

        adminGroup.MapGet("/machines/{id:guid}",
            async (Guid id, IMediator m, CancellationToken ct) =>
                Results.Ok(await m.Send(new GetMachineDetailQuery(id), ct)))
            .WithName("GetMachineDetail")
            .RequirePermission(MachinesPermissions.Machines.View);

        adminGroup.MapGet("/machines/{id:guid}/metrics",
            async (Guid id, int days, IMediator m, CancellationToken ct) =>
                Results.Ok(await m.Send(new GetMachineMetricsQuery(id, days), ct)))
            .WithName("GetMachineMetrics")
            .RequirePermission(MachinesPermissions.Machines.View);

        adminGroup.MapPost("/machines/{id:guid}/commands",
            async (Guid id, SendMachineCommandRequest req, IMediator m, CancellationToken ct) =>
                Results.Ok(await m.Send(new SendMachineCommandCommand(id, req.Type, req.PayloadJson), ct)))
            .WithName("SendMachineCommand")
            .RequirePermission(MachinesPermissions.Machines.Command);
    }
}

// Request DTOs for endpoints that need them (not using Mediator commands directly for body parsing)
public sealed record PostCommandResultRequest(bool Success, string? OutputJson, string? ErrorMessage);
public sealed record SendMachineCommandRequest(MIT.Modules.Machines.Contracts.Dtos.CommandType Type, string PayloadJson);
