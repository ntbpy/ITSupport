using MIT.Framework.Web;
using MIT.Framework.Web.Modules;
using MIT.Modules.Auditing;
using MIT.Modules.Identity;
using MIT.Modules.Identity.Contracts.v1.Tokens.TokenGeneration;
using MIT.Modules.Identity.Features.v1.Tokens.TokenGeneration;
using MIT.Modules.Multitenancy;
using MIT.Modules.Multitenancy.Contracts.v1.GetTenantStatus;
using MIT.Modules.Webhooks;
using MIT.Modules.Billing;
using MIT.Modules.Catalog;
using MIT.Modules.Tickets;
using MIT.Modules.Machines;
using MIT.Modules.Multitenancy.Features.v1.GetTenantStatus;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Serialize enums as string names (reads still accept names or integers). [Flags] enums (AuditTag, BodyCapture)
// opt back to numeric via their own NumericEnumConverter since comma-joined flag strings break bitwise consumers. Frontends mirror this as string unions.
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

if (builder.Environment.IsProduction())
{
    static void Require(IConfiguration config, string key)
    {
        if (string.IsNullOrWhiteSpace(config[key]))
        {
            throw new InvalidOperationException($"Missing required configuration '{key}' in Production.");
        }
    }

    var config = builder.Configuration;
    Require(config, "DatabaseOptions:ConnectionString");
    Require(config, "CachingOptions:Redis");
    Require(config, "JwtOptions:SigningKey");
}

builder.Services.AddMediator(o =>
{
    o.ServiceLifetime = ServiceLifetime.Scoped;
    o.Assemblies = [
        typeof(GenerateTokenCommand),
        typeof(GenerateTokenCommandHandler),
        typeof(GetTenantStatusQuery),
        typeof(GetTenantStatusQueryHandler),
        typeof(MIT.Modules.Auditing.Contracts.AuditEnvelope),
        typeof(MIT.Modules.Auditing.Persistence.AuditDbContext),
        typeof(MIT.Modules.Webhooks.Contracts.v1.CreateWebhookSubscription.CreateWebhookSubscriptionCommand),
        typeof(MIT.Modules.Webhooks.WebhooksModule),
        typeof(MIT.Modules.Billing.Contracts.BillingContractsMarker),
        typeof(MIT.Modules.Billing.BillingModule),
        typeof(MIT.Modules.Catalog.Contracts.CatalogContractsMarker),
        typeof(MIT.Modules.Catalog.CatalogModule),
        typeof(MIT.Modules.Tickets.Contracts.TicketsContractsMarker),
        typeof(MIT.Modules.Tickets.TicketsModule),
        typeof(MIT.Modules.Files.Contracts.v1.Commands.RequestUploadUrlCommand),
        typeof(MIT.Modules.Files.FilesModule),
        typeof(MIT.Modules.Chat.Contracts.v1.Commands.CreateChannelCommand),
        typeof(MIT.Modules.Chat.ChatModule),
        typeof(MIT.Modules.Notifications.Contracts.v1.Commands.MarkNotificationReadCommand),
        typeof(MIT.Modules.Notifications.NotificationsModule),
        typeof(MIT.Modules.Machines.Contracts.MachinesContractsMarker),
        typeof(MIT.Modules.Machines.MachinesModule),
        typeof(MIT.Modules.Diagnostics.Contracts.DiagnosticsContractsMarker),
        typeof(MIT.Modules.Diagnostics.DiagnosticsModule),
        typeof(MIT.Modules.Alerts.Contracts.AlertsContractsMarker),
        typeof(MIT.Modules.Alerts.AlertsModule)];
});

var moduleAssemblies = new Assembly[]
{
    typeof(IdentityModule).Assembly,
    typeof(MultitenancyModule).Assembly,
    typeof(AuditingModule).Assembly,
    typeof(MIT.Modules.Files.FilesModule).Assembly,
    typeof(WebhooksModule).Assembly,
    typeof(BillingModule).Assembly,
    typeof(CatalogModule).Assembly,
    typeof(TicketsModule).Assembly,
    typeof(MIT.Modules.Chat.ChatModule).Assembly,
    typeof(MIT.Modules.Notifications.NotificationsModule).Assembly,
    typeof(MIT.Modules.Machines.MachinesModule).Assembly,
    typeof(MIT.Modules.Diagnostics.DiagnosticsModule).Assembly,
    typeof(MIT.Modules.Alerts.AlertsModule).Assembly,
};

builder.AddHeroPlatform(o =>
{
    o.EnableCaching = true;
    o.EnableMailing = true;
    o.EnableJobs = true;
    o.EnableQuotas = true;
    o.EnableSse = true;
    o.EnableRealtime = true;
});

// Register real SignalR publishers before modules so TryAdd* in modules picks the NoOp only as fallback.
builder.Services.AddSingleton<MIT.Modules.Diagnostics.Infrastructure.IDiagnosticPublisher,
    MIT.Starter.Api.Hubs.Publishers.DiagnosticPublisher>();
builder.Services.AddSingleton<MIT.Starter.Api.Hubs.Publishers.IMachineStatusPublisher,
    MIT.Starter.Api.Hubs.Publishers.MachineStatusPublisher>();
builder.Services.AddSingleton<MIT.Starter.Api.Hubs.Publishers.ICommandPublisher,
    MIT.Starter.Api.Hubs.Publishers.CommandPublisher>();
builder.Services.AddSingleton<MIT.Starter.Api.Hubs.Publishers.ITicketPublisher,
    MIT.Starter.Api.Hubs.Publishers.TicketPublisher>();

builder.AddModules(moduleAssemblies);

// Self-heal deployments carrying retired per-module `{module}-outbox-dispatcher` Hangfire recurring jobs
// (the outbox is now dispatched by OutboxDispatcherHostedService). No-op once the storage is clean.
builder.Services.AddHostedService<MIT.Starter.Api.OrphanedOutboxRecurringJobCleanupService>();

// Demo data is provisioned by the DbMigrator's `seed-demo` verb, not the API — the API never mutates data on startup.
// See src/Host/MIT.Starter.DbMigrator/README.md.

var app = builder.Build();

app.UseHeroMultiTenantDatabases();
app.UseHeroPlatform(p =>
{
    p.MapModules = true;
    p.ServeStaticFiles = true;
    p.UseQuotas = true;
    p.MapSseEndpoints = true;
    p.MapRealtime = true;
});

// VietRMM SignalR hubs
app.MapHub<MIT.Starter.Api.Hubs.MachineStatusHub>("/hubs/machine-status");
app.MapHub<MIT.Starter.Api.Hubs.DiagnosticHub>("/hubs/diagnostic");
app.MapHub<MIT.Starter.Api.Hubs.CommandHub>("/hubs/command");
app.MapHub<MIT.Starter.Api.Hubs.TicketHub>("/hubs/ticket");
app.MapHub<MIT.Starter.Api.Hubs.RemoteSessionHub>("/hubs/remote-session");
app.MapHub<MIT.Starter.Api.Hubs.BuildHub>("/hubs/build");

app.MapGet("/", () => Results.Ok(new { message = "hello world!" }))
   .WithTags("PlayGround")
   .AllowAnonymous();
await app.RunAsync();