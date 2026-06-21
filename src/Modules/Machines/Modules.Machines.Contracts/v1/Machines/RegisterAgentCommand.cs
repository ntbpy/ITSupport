using Mediator;

namespace MIT.Modules.Machines.Contracts.v1.Machines;

public sealed record RegisterAgentCommand(
    string MachineName,
    string MacAddress,
    string IpAddress,
    string? OsVersion,
    string? CpuModel,
    decimal RamGb,
    decimal DiskTotalGb,
    string? AgentVersion) : ICommand<RegisterAgentResponse>;

public sealed record RegisterAgentResponse(Guid MachineId, string ApiKey, DateTime ServerTime);
