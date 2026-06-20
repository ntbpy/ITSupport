using Asp.Versioning;
using MIT.Framework.Persistence;
using MIT.Framework.Shared.Constants;
using MIT.Framework.Web.HttpResilience;
using MIT.Framework.Web.Modules;
using MIT.Modules.Webhooks.Contracts.Authorization;
using MIT.Modules.Webhooks.Data;
using MIT.Modules.Webhooks.Features.v1.CreateWebhookSubscription;
using MIT.Modules.Webhooks.Features.v1.DeleteWebhookSubscription;
using MIT.Modules.Webhooks.Features.v1.GetWebhookDeliveries;
using MIT.Modules.Webhooks.Features.v1.GetWebhookSubscriptions;
using MIT.Modules.Webhooks.Features.v1.TestWebhookSubscription;
using MIT.Framework.Eventing.Abstractions;
using MIT.Modules.Webhooks.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

[assembly: FshModule(typeof(MIT.Modules.Webhooks.WebhooksModule), 400)]

namespace MIT.Modules.Webhooks;

public sealed class WebhooksModule : IModule
{
    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        PermissionConstants.Register(WebhooksPermissions.All);

        builder.Services.AddHeroDbContext<WebhookDbContext>();
        builder.Services.AddScoped<IDbInitializer, WebhookDbInitializer>();
        builder.Services.AddSingleton<IWebhookSecretProtector, WebhookSecretProtector>();
        builder.Services.AddScoped<IWebhookDeliveryService, WebhookDeliveryService>();
        builder.Services.AddScoped<IWebhookDispatcher, WebhookDispatcher>();
        builder.Services.AddScoped<WebhookDispatchJob>();

        // Open-generic integration-event bridge — every published IIntegrationEvent fans out to
        // matching tenant webhook subscriptions; DI materializes closed handler types per event.
        builder.Services.AddScoped(
            typeof(IIntegrationEventHandler<>),
            typeof(WebhookFanoutHandler<>));

        builder.Services.AddHttpClient("Webhooks")
            .AddHeroResilience(builder.Configuration);

        builder.Services.AddHealthChecks()
            .AddDbContextCheck<WebhookDbContext>(
                name: "db:webhooks",
                failureStatus: HealthStatus.Unhealthy);
    }

    public void ConfigureMiddleware(Microsoft.AspNetCore.Builder.IApplicationBuilder app)
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
            .MapGroup("api/v{version:apiVersion}/webhooks")
            .WithTags("Webhooks")
            .WithApiVersionSet(versionSet)
            .RequireAuthorization();

        group.MapCreateWebhookSubscriptionEndpoint();
        group.MapDeleteWebhookSubscriptionEndpoint();
        group.MapGetWebhookSubscriptionsEndpoint();
        group.MapGetWebhookDeliveriesEndpoint();
        group.MapTestWebhookSubscriptionEndpoint();
    }
}
