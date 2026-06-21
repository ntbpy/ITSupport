using StackExchange.Redis;

namespace MIT.Modules.Diagnostics.Infrastructure;

public sealed class DiagnosticRateLimiter(IConnectionMultiplexer redis)
{
    // Basic plan: 5 diagnostics/machine/day. Pro+Enterprise: unlimited.
    public async Task<bool> AllowAsync(
        Guid tenantId, Guid machineId, CancellationToken ct = default)
    {
        var key = $"diag_rl:{tenantId}:{machineId}:{DateTime.UtcNow:yyyy-MM-dd}";
        var db = redis.GetDatabase();
        var count = await db.StringIncrementAsync(key).ConfigureAwait(false);
        if (count == 1)
            await db.KeyExpireAsync(key, TimeSpan.FromHours(25)).ConfigureAwait(false);
        return count <= 5;
    }
}
