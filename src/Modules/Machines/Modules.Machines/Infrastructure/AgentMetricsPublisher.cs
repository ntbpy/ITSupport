using MIT.Modules.Machines.Contracts.v1.Machines;
using MIT.Modules.Machines.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace MIT.Modules.Machines.Infrastructure;

public interface IAgentMetricsPublisher
{
    Task PublishHeartbeatAsync(HeartbeatCommand cmd, CancellationToken ct = default);
    Task PublishSnapshotAsync(PostSnapshotCommand cmd, CancellationToken ct = default);
}

public sealed partial class AgentMetricsPublisher(
    MachinesDbContext dbContext,
    IConnectionMultiplexer redis,
    ILogger<AgentMetricsPublisher> logger) : IAgentMetricsPublisher
{
    public async Task PublishHeartbeatAsync(HeartbeatCommand cmd, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(cmd);

        await dbContext.Database.ExecuteSqlRawAsync(
            """
            INSERT INTO machines.system_snapshots
              (time, machine_id, cpu_usage_pct, ram_used_gb, disk_used_gb, disk_free_pct,
               network_in_mbps, network_out_mbps, event_log_errors, top_processes_json)
            VALUES (NOW(), {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}::jsonb)
            """,
            [cmd.MachineId, cmd.CpuUsagePct, cmd.RamUsedGb, cmd.DiskUsedGb, cmd.DiskFreePct,
             cmd.NetworkInMbps, cmd.NetworkOutMbps, cmd.EventLogErrors,
             cmd.TopProcessesJson ?? "[]"], ct)
            .ConfigureAwait(false);

        var db = redis.GetDatabase();
        var payload = JsonSerializer.Serialize(new
        {
            machineId = cmd.MachineId,
            cpuPct = cmd.CpuUsagePct,
            ramGb = cmd.RamUsedGb,
            diskFreePct = cmd.DiskFreePct,
        });
        await db.PublishAsync(
            RedisChannel.Literal($"metrics:{cmd.MachineId}"), payload).ConfigureAwait(false);

        LogHeartbeatPublished(logger, cmd.MachineId);
    }

    public async Task PublishSnapshotAsync(PostSnapshotCommand cmd, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(cmd);

        await dbContext.Database.ExecuteSqlRawAsync(
            """
            UPDATE machines.system_snapshots
            SET hardware_json = {1}::jsonb, software_list_json = {2}::jsonb
            WHERE machine_id = {0}
              AND time = (SELECT MAX(time) FROM machines.system_snapshots WHERE machine_id = {0})
            """,
            [cmd.MachineId, cmd.HardwareJson, cmd.SoftwareListJson], ct)
            .ConfigureAwait(false);

        var db = redis.GetDatabase();
        await db.StreamAddAsync(
            "ai_diagnostic_queue",
            [new NameValueEntry("machineId", cmd.MachineId.ToString())]).ConfigureAwait(false);
    }

    [LoggerMessage(Level = LogLevel.Debug, Message = "Heartbeat published for machine {MachineId}")]
    private static partial void LogHeartbeatPublished(ILogger logger, Guid machineId);
}
