using Finbuckle.MultiTenant.Abstractions;
using MIT.Framework.Persistence.Context;
using MIT.Framework.Shared.Multitenancy;
using MIT.Framework.Shared.Persistence;
using MIT.Modules.Machines.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace MIT.Modules.Machines.Data;

public sealed class MachinesDbContext : BaseDbContext
{
    public const string Schema = "machines";

    public MachinesDbContext(
        IMultiTenantContextAccessor<AppTenantInfo> multiTenantContextAccessor,
        DbContextOptions<MachinesDbContext> options,
        IOptions<DatabaseOptions> settings,
        IHostEnvironment environment)
        : base(multiTenantContextAccessor, options, settings, environment) { }

    public DbSet<Machine> Machines => Set<Machine>();
    public DbSet<MachineGroup> MachineGroups => Set<MachineGroup>();
    public DbSet<MachineCommand> MachineCommands => Set<MachineCommand>();
    public DbSet<PatchStatus> PatchStatuses => Set<PatchStatus>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MachinesDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
