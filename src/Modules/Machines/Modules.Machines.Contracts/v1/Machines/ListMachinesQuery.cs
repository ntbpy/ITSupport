using Mediator;
using MIT.Framework.Shared.Persistence;
using MIT.Modules.Machines.Contracts.Dtos;

namespace MIT.Modules.Machines.Contracts.v1.Machines;

public sealed record ListMachinesQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    MachineStatus? Status = null) : IQuery<PagedResponse<MachineDto>>;
