using Finbuckle.MultiTenant.Abstractions;
using MIT.Framework.Persistence.Context;
using MIT.Framework.Shared.Multitenancy;
using MIT.Framework.Shared.Persistence;
using MIT.Modules.Diagnostics.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace MIT.Modules.Diagnostics.Data;

public sealed class DiagnosticsDbContext : BaseDbContext
{
    public DiagnosticsDbContext(
        IMultiTenantContextAccessor<AppTenantInfo> multiTenantContextAccessor,
        DbContextOptions<DiagnosticsDbContext> options,
        IOptions<DatabaseOptions> settings,
        IHostEnvironment environment)
        : base(multiTenantContextAccessor, options, settings, environment) { }

    public DbSet<DiagnosticReport> DiagnosticReports => Set<DiagnosticReport>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        modelBuilder.HasDefaultSchema("diagnostics");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DiagnosticsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
