using Mediator;

namespace MIT.Modules.Machines.Contracts.v1.Machines;

public sealed record PostCommandResultCommand(
    Guid MachineId,
    Guid CommandId,
    bool Success,
    string? OutputJson,
    string? ErrorMessage) : ICommand;
