using Mediator;
using MIT.Modules.Machines.Contracts.Dtos;

namespace MIT.Modules.Machines.Contracts.v1.Machines;

public sealed record SendMachineCommandCommand(
    Guid MachineId,
    CommandType Type,
    string PayloadJson) : ICommand<Guid>;
