using MIT.Modules.Machines.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MIT.Modules.Machines.Data.Configurations;

public sealed class SystemSnapshotConfiguration : IEntityTypeConfiguration<SystemSnapshot>
{
    public void Configure(EntityTypeBuilder<SystemSnapshot> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("system_snapshots");
        // Hypertable has composite PK (Time, MachineId) — EF maps it as keyless for reads.
        // Inserts done via raw INSERT SQL in AgentMetricsPublisher.
        builder.HasNoKey();
        builder.Property(x => x.Time).HasColumnName("time").IsRequired();
        builder.Property(x => x.MachineId).HasColumnName("machine_id").IsRequired();
        builder.Property(x => x.CpuUsagePct).HasColumnName("cpu_usage_pct").HasPrecision(5, 2);
        builder.Property(x => x.RamUsedGb).HasColumnName("ram_used_gb").HasPrecision(5, 1);
        builder.Property(x => x.DiskUsedGb).HasColumnName("disk_used_gb").HasPrecision(8, 1);
        builder.Property(x => x.DiskFreePct).HasColumnName("disk_free_pct").HasPrecision(5, 2);
        builder.Property(x => x.NetworkInMbps).HasColumnName("network_in_mbps").HasPrecision(8, 2);
        builder.Property(x => x.NetworkOutMbps).HasColumnName("network_out_mbps").HasPrecision(8, 2);
        builder.Property(x => x.TopProcessesJson).HasColumnName("top_processes_json").HasColumnType("jsonb");
        builder.Property(x => x.EventLogErrors).HasColumnName("event_log_errors");
        builder.Property(x => x.HardwareJson).HasColumnName("hardware_json").HasColumnType("jsonb");
        builder.Property(x => x.SoftwareListJson).HasColumnName("software_list_json").HasColumnType("jsonb");
    }
}
