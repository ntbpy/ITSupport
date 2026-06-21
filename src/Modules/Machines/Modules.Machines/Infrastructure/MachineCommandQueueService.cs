using MIT.Modules.Machines.Contracts.v1.Machines;
using StackExchange.Redis;
using System.Text.Json;

namespace MIT.Modules.Machines.Infrastructure;

public interface IMachineCommandQueueService
{
    Task EnqueueAsync(Guid machineId, string commandJson, CancellationToken ct = default);
    Task<IReadOnlyList<PendingCommandDto>> DequeueAsync(Guid machineId, CancellationToken ct = default);
    Task<bool> HasCommandsAsync(Guid machineId, CancellationToken ct = default);
}

public sealed class MachineCommandQueueService(IConnectionMultiplexer redis) : IMachineCommandQueueService
{
    private static string Key(Guid id) => $"commands:{id}";

    public async Task EnqueueAsync(Guid machineId, string commandJson, CancellationToken ct = default)
    {
        var db = redis.GetDatabase();
        await db.ListRightPushAsync(Key(machineId), commandJson).ConfigureAwait(false);
    }

    public async Task<bool> HasCommandsAsync(Guid machineId, CancellationToken ct = default)
    {
        var db = redis.GetDatabase();
        return await db.ListLengthAsync(Key(machineId)).ConfigureAwait(false) > 0;
    }

    public async Task<IReadOnlyList<PendingCommandDto>> DequeueAsync(
        Guid machineId, CancellationToken ct = default)
    {
        var db = redis.GetDatabase();
        var items = await db.ListRangeAsync(Key(machineId)).ConfigureAwait(false);
        await db.KeyDeleteAsync(Key(machineId)).ConfigureAwait(false);
        return items
            .Select(i => JsonSerializer.Deserialize<PendingCommandDto>(i.ToString())!)
            .ToList()
            .AsReadOnly();
    }
}
