using Mediator;

namespace MIT.Modules.Machines.Contracts.v1.Machines;

public sealed record HeartbeatCommand(
    Guid MachineId,
    decimal CpuUsagePct,
    decimal RamUsedGb,
    decimal DiskUsedGb,
    decimal DiskFreePct,
    decimal NetworkInMbps,
    decimal NetworkOutMbps,
    int EventLogErrors,
    string? TopProcessesJson) : ICommand<HeartbeatResponse>;

public sealed record HeartbeatResponse(bool HasCommands, DateTime ServerTime);
