using MIT.Modules.Machines.Contracts.Dtos;
using MIT.Modules.Machines.Contracts.v1.Machines;
using MIT.Modules.Machines.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace MIT.Modules.Machines.Features.v1.Admin.GetMachineMetrics;

public sealed class GetMachineMetricsQueryHandler(MachinesDbContext dbContext)
    : IQueryHandler<GetMachineMetricsQuery, IReadOnlyList<MachineMetricDto>>
{
    public async ValueTask<IReadOnlyList<MachineMetricDto>> Handle(
        GetMachineMetricsQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var cutoff = DateTime.UtcNow.AddDays(-query.Days);
        var items = await dbContext.SystemSnapshots
            .FromSqlRaw(
                """
                SELECT time_bucket('1 hour', time) AS time,
                       machine_id,
                       AVG(cpu_usage_pct)     AS cpu_usage_pct,
                       AVG(ram_used_gb)       AS ram_used_gb,
                       AVG(disk_used_gb)      AS disk_used_gb,
                       AVG(disk_free_pct)     AS disk_free_pct,
                       AVG(network_in_mbps)   AS network_in_mbps,
                       AVG(network_out_mbps)  AS network_out_mbps,
                       SUM(event_log_errors)  AS event_log_errors,
                       NULL::jsonb            AS top_processes_json,
                       NULL::jsonb            AS hardware_json,
                       NULL::jsonb            AS software_list_json
                FROM machines.system_snapshots
                WHERE machine_id = {0} AND time >= {1}
                GROUP BY 1, 2 ORDER BY 1
                """, query.MachineId, cutoff)
            .AsNoTracking()
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return items.Select(s => new MachineMetricDto(
            s.Time, s.CpuUsagePct, s.RamUsedGb, s.DiskFreePct,
            s.NetworkInMbps, s.NetworkOutMbps, s.EventLogErrors)).ToList();
    }
}
