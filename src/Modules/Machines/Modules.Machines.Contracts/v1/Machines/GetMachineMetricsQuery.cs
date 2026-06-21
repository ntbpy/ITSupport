using Mediator;
using MIT.Modules.Machines.Contracts.Dtos;

namespace MIT.Modules.Machines.Contracts.v1.Machines;

public sealed record GetMachineMetricsQuery(Guid MachineId, int Days = 7) : IQuery<IReadOnlyList<MachineMetricDto>>;
