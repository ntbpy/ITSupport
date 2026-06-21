using MIT.Framework.Shared.Persistence;
using MIT.Modules.Machines.Contracts.Dtos;
using MIT.Modules.Machines.Contracts.v1.Machines;
using MIT.Modules.Machines.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace MIT.Modules.Machines.Features.v1.Admin.ListMachines;

public sealed class ListMachinesQueryHandler(MachinesDbContext dbContext)
    : IQueryHandler<ListMachinesQuery, PagedResponse<MachineDto>>
{
    public async ValueTask<PagedResponse<MachineDto>> Handle(
        ListMachinesQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        int page = query.Page < 1 ? 1 : query.Page;
        int size = query.PageSize is < 1 or > 200 ? 20 : query.PageSize;

        var q = dbContext.Machines.AsNoTracking();

        if (query.Status is not null)
            q = q.Where(m => m.Status == query.Status);
        if (!string.IsNullOrWhiteSpace(query.Search))
            q = q.Where(m => m.MachineName.Contains(query.Search)
                           || m.IpAddress.Contains(query.Search));

        var total = await q.LongCountAsync(cancellationToken).ConfigureAwait(false);
        var items = await q
            .OrderBy(m => m.MachineName)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(m => new MachineDto(m.Id, m.MachineName, m.IpAddress, m.MacAddress,
                m.OsVersion, m.CpuModel, m.RamGb, m.DiskTotalGb, m.AgentVersion,
                m.LastSeenAt, m.Status, m.AssignedUser, m.GroupId, m.Notes))
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        return new PagedResponse<MachineDto>
        {
            Items = items,
            PageNumber = page,
            PageSize = size,
            TotalCount = total,
            TotalPages = (int)Math.Ceiling(total / (double)size),
        };
    }
}
