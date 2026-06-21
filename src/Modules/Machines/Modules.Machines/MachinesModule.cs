using Asp.Versioning;
using MIT.Framework.Persistence;
using MIT.Framework.Web.Modules;
using MIT.Modules.Machines.Data;
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
        builder.Services.AddHeroDbContext<MachinesDbContext>();
        builder.Services.AddScoped<IDbInitializer, MachinesDbInitializer>();
        builder.Services.AddHealthChecks()
            .AddDbContextCheck<MachinesDbContext>(
                name: "db:machines",
                failureStatus: HealthStatus.Unhealthy);
    }

    public void ConfigureMiddleware(IApplicationBuilder app) { }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        var versionSet = endpoints.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();

        endpoints
            .MapGroup("api/v{version:apiVersion}")
            .WithTags("Machines")
            .WithApiVersionSet(versionSet);
    }
}
