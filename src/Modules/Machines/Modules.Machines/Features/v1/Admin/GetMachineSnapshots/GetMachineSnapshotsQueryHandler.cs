using MIT.Modules.Machines.Contracts.Dtos;
using MIT.Modules.Machines.Contracts.v1.Machines;
using MIT.Modules.Machines.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace MIT.Modules.Machines.Features.v1.Admin.GetMachineSnapshots;

public sealed class GetMachineSnapshotsQueryHandler(MachinesDbContext dbContext)
    : IQueryHandler<GetMachineSnapshotsQuery, IReadOnlyList<MachineSnapshotDto>>
{
    public async ValueTask<IReadOnlyList<MachineSnapshotDto>> Handle(
        GetMachineSnapshotsQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var cutoff = DateTime.UtcNow.AddDays(-query.DaysBack);
        var items = await dbContext.SystemSnapshots
            .FromSqlRaw(
                "SELECT * FROM machines.system_snapshots WHERE machine_id = {0} AND time >= {1} ORDER BY time",
                query.MachineId, cutoff)
            .AsNoTracking()
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        return items.Select(s => new MachineSnapshotDto(
            s.Time, s.CpuUsagePct, s.RamUsedGb, s.DiskUsedGb, s.DiskFreePct,
            s.NetworkInMbps, s.NetworkOutMbps, s.EventLogErrors, s.SoftwareListJson))
            .ToList();
    }
}
