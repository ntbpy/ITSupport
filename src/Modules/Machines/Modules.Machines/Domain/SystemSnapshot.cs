namespace MIT.Modules.Machines.Domain;

// Read-only projection from the TimescaleDB hypertable system_snapshots.
// Not an AggregateRoot — inserts happen via raw SQL in AgentMetricsPublisher.
public sealed class SystemSnapshot
{
    public DateTime Time { get; init; }
    public Guid MachineId { get; init; }
    public decimal CpuUsagePct { get; init; }
    public decimal RamUsedGb { get; init; }
    public decimal DiskUsedGb { get; init; }
    public decimal DiskFreePct { get; init; }
    public decimal NetworkInMbps { get; init; }
    public decimal NetworkOutMbps { get; init; }
    public string? TopProcessesJson { get; init; }
    public int EventLogErrors { get; init; }
    public string? HardwareJson { get; init; }
    public string? SoftwareListJson { get; init; }
}
