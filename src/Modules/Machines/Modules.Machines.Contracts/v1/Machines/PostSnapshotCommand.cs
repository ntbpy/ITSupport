using Mediator;

namespace MIT.Modules.Machines.Contracts.v1.Machines;

public sealed record PostSnapshotCommand(
    Guid MachineId,
    string HardwareJson,
    string SoftwareListJson,
    string WindowsUpdatePendingJson,
    string AntivirusStatusJson,
    bool FirewallEnabled) : ICommand;
