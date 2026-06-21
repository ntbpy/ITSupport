using MIT.Modules.Machines.Contracts.Dtos;
using MIT.Modules.Machines.Contracts.v1.Machines;
using MIT.Modules.Machines.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace MIT.Modules.Machines.Features.v1.Admin.GetMachineDetail;

public sealed class GetMachineDetailQueryHandler(MachinesDbContext dbContext)
    : IQueryHandler<GetMachineDetailQuery, MachineDto>
{
    public async ValueTask<MachineDto> Handle(
        GetMachineDetailQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var m = await dbContext.Machines
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == query.MachineId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new KeyNotFoundException($"Machine {query.MachineId} not found");

        return new MachineDto(m.Id, m.MachineName, m.IpAddress, m.MacAddress,
            m.OsVersion, m.CpuModel, m.RamGb, m.DiskTotalGb, m.AgentVersion,
            m.LastSeenAt, m.Status, m.AssignedUser, m.GroupId, m.Notes);
    }
}
