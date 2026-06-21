using Asp.Versioning;
using MIT.Framework.Persistence;
using MIT.Framework.Shared.Constants;
using MIT.Framework.Shared.Identity.Authorization;
using MIT.Framework.Web.Modules;
using MIT.Modules.Alerts.Contracts.Authorization;
using MIT.Modules.Alerts.Contracts.v1.Alerts;
using MIT.Modules.Alerts.Data;
using MIT.Modules.Alerts.Infrastructure;
using MIT.Modules.Alerts.Workers;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

[assembly: FshModule(typeof(MIT.Modules.Alerts.AlertsModule), 820)]

namespace MIT.Modules.Alerts;

public sealed class AlertsModule : IModule
{
    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        PermissionConstants.Register(AlertsPermissions.All);

        builder.Services.AddHeroDbContext<AlertsDbContext>();
        builder.Services.AddScoped<IDbInitializer, AlertsDbInitializer>();
        builder.Services.AddHealthChecks()
            .AddDbContextCheck<AlertsDbContext>(
                name: "db:alerts",
                failureStatus: HealthStatus.Unhealthy);

        builder.Services.Configure<ZaloOAOptions>(
            builder.Configuration.GetSection(ZaloOAOptions.Section));
        builder.Services.AddHttpClient<ZaloOAService>(client =>
        {
            var baseUrl = builder.Configuration["ZaloOA:BaseUrl"] ?? "https://openapi.zalo.me/";
            client.BaseAddress = new Uri(baseUrl);
        });
        builder.Services.AddScoped<AlertDeduplicationService>();
        builder.Services.AddScoped<AlertNotificationOrchestrator>();
        builder.Services.AddHostedService<AlertEngineWorker>();
    }

    public void ConfigureMiddleware(IApplicationBuilder app) { }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        var versionSet = endpoints.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();

        var group = endpoints
            .MapGroup("api/v{version:apiVersion}/admin")
            .WithTags("Alerts")
            .WithApiVersionSet(versionSet)
            .RequireAuthorization();

        group.MapGet("/alerts",
            async ([AsParameters] ListAlertsQuery q, IMediator m, CancellationToken ct) =>
                Results.Ok(await m.Send(q, ct)))
            .WithName("ListAlerts")
            .RequirePermission(AlertsPermissions.Alerts.View);

        group.MapPost("/alerts/{alertId:guid}/acknowledge",
            async (Guid alertId, IMediator m, CancellationToken ct) =>
            {
                await m.Send(new AcknowledgeAlertCommand(alertId), ct);
                return Results.Ok();
            })
            .WithName("AcknowledgeAlert")
            .RequirePermission(AlertsPermissions.Alerts.Acknowledge);
    }
}
