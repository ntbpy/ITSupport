using Mediator;

namespace MIT.Modules.Machines.Contracts.v1.Machines;

public sealed record GetCommandsQuery(Guid MachineId) : IQuery<IReadOnlyList<PendingCommandDto>>;

public sealed record PendingCommandDto(Guid CommandId, string Type, string PayloadJson);
