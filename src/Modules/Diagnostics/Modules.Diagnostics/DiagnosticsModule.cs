using Asp.Versioning;
using MIT.Framework.Persistence;
using MIT.Framework.Shared.Constants;
using MIT.Framework.Shared.Identity.Authorization;
using MIT.Framework.Web.Modules;
using MIT.Modules.Diagnostics.Contracts.Authorization;
using MIT.Modules.Diagnostics.Contracts.v1.Diagnostics;
using MIT.Modules.Diagnostics.Data;
using MIT.Modules.Diagnostics.Infrastructure;
using MIT.Modules.Diagnostics.Workers;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

[assembly: FshModule(typeof(MIT.Modules.Diagnostics.DiagnosticsModule), 810)]

namespace MIT.Modules.Diagnostics;

public sealed class DiagnosticsModule : IModule
{
    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        PermissionConstants.Register(DiagnosticsPermissions.All);

        builder.Services.AddHeroDbContext<DiagnosticsDbContext>();
        builder.Services.AddScoped<IDbInitializer, DiagnosticsDbInitializer>();
        builder.Services.AddHealthChecks()
            .AddDbContextCheck<DiagnosticsDbContext>(
                name: "db:diagnostics",
                failureStatus: HealthStatus.Unhealthy);

        builder.Services.Configure<ClaudeApiOptions>(
            builder.Configuration.GetSection(ClaudeApiOptions.Section));
        builder.Services.AddHttpClient<IClaudeApiService, ClaudeApiService>(client =>
        {
            var baseUrl = builder.Configuration["ClaudeApi:BaseUrl"] ?? "https://api.anthropic.com/";
            client.BaseAddress = new Uri(baseUrl);
            var apiKey = builder.Configuration["ClaudeApi:ApiKey"] ?? string.Empty;
            client.DefaultRequestHeaders.Add("x-api-key", apiKey);
            client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
        });

        // DiagnosticPromptBuilder and DiagnosticResultParser are static — no registration needed.
        builder.Services.AddScoped<DiagnosticRateLimiter>();
        builder.Services.AddScoped<AutoFixService>();
        // IDiagnosticPublisher is registered by the host (DiagnosticPublisher → DiagnosticHub).
        // NoOpDiagnosticPublisher is used when the module runs standalone (e.g. tests).
        builder.Services.TryAddSingleton<IDiagnosticPublisher, NoOpDiagnosticPublisher>();
        builder.Services.AddHostedService<AiDiagnosticWorker>();
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
            .WithTags("Diagnostics")
            .WithApiVersionSet(versionSet)
            .RequireAuthorization();

        group.MapPost("/machines/{machineId:guid}/diagnostic",
            async (Guid machineId, IMediator m, CancellationToken ct) =>
                Results.Ok(await m.Send(new TriggerDiagnosticCommand(machineId), ct)))
            .WithName("TriggerDiagnostic")
            .RequirePermission(DiagnosticsPermissions.Diagnostics.Trigger);

        group.MapGet("/machines/{machineId:guid}/diagnostics",
            async (Guid machineId, int page, int pageSize, IMediator m, CancellationToken ct) =>
                Results.Ok(await m.Send(new ListDiagnosticsQuery(machineId, page, pageSize), ct)))
            .WithName("ListDiagnostics")
            .RequirePermission(DiagnosticsPermissions.Diagnostics.View);

        group.MapPost("/diagnostics/{reportId:guid}/acknowledge",
            async (Guid reportId, IMediator m, CancellationToken ct) =>
            {
                await m.Send(new AcknowledgeDiagnosticCommand(reportId), ct);
                return Results.Ok();
            })
            .WithName("AcknowledgeDiagnostic")
            .RequirePermission(DiagnosticsPermissions.Diagnostics.View);
    }
}
