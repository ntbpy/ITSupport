using Finbuckle.MultiTenant.Abstractions;
using MIT.Framework.Persistence.Context;
using MIT.Framework.Shared.Multitenancy;
using MIT.Framework.Shared.Persistence;
using MIT.Modules.Alerts.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace MIT.Modules.Alerts.Data;

public sealed class AlertsDbContext : BaseDbContext
{
    public AlertsDbContext(
        IMultiTenantContextAccessor<AppTenantInfo> multiTenantContextAccessor,
        DbContextOptions<AlertsDbContext> options,
        IOptions<DatabaseOptions> settings,
        IHostEnvironment environment)
        : base(multiTenantContextAccessor, options, settings, environment) { }

    public DbSet<Alert> Alerts => Set<Alert>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        modelBuilder.HasDefaultSchema("alerts");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AlertsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
