namespace MIT.Modules.Machines.Contracts.Dtos;

public sealed record MachineSnapshotDto(
    DateTime Time,
    decimal CpuUsagePct,
    decimal RamUsedGb,
    decimal DiskUsedGb,
    decimal DiskFreePct,
    decimal NetworkInMbps,
    decimal NetworkOutMbps,
    int EventLogErrors,
    string? SoftwareListJson);
