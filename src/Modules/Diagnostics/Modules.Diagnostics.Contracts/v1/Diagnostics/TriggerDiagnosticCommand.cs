using Mediator;

namespace MIT.Modules.Diagnostics.Contracts.v1.Diagnostics;

public sealed record TriggerDiagnosticCommand(Guid MachineId) : ICommand<Guid>;
