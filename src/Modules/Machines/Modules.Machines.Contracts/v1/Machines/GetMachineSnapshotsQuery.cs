using Mediator;
using MIT.Modules.Machines.Contracts.Dtos;

namespace MIT.Modules.Machines.Contracts.v1.Machines;

public sealed record GetMachineSnapshotsQuery(Guid MachineId, int DaysBack = 7)
    : IQuery<IReadOnlyList<MachineSnapshotDto>>;
