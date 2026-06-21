namespace MIT.Modules.Machines.Contracts.Dtos;

public sealed record MachineDto(
    Guid Id, string MachineName, string IpAddress, string MacAddress,
    string? OsVersion, string? CpuModel, decimal RamGb, decimal DiskTotalGb,
    string? AgentVersion, DateTime? LastSeenAt, MachineStatus Status,
    string? AssignedUser, Guid? GroupId, string? Notes);

public sealed record MachineMetricDto(
    DateTime Time, decimal CpuPct, decimal RamGb, decimal DiskFreePct,
    decimal NetworkInMbps, decimal NetworkOutMbps, int EventLogErrors);
