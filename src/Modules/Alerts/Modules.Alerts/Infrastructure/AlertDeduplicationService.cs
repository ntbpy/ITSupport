using StackExchange.Redis;

namespace MIT.Modules.Alerts.Infrastructure;

public sealed class AlertDeduplicationService(IConnectionMultiplexer redis)
{
    private static readonly Dictionary<string, TimeSpan> Cooldowns = new()
    {
        ["MACHINE_OFFLINE"]    = TimeSpan.FromMinutes(30),
        ["CPU_HIGH"]           = TimeSpan.FromHours(1),
        ["RAM_CRITICAL"]       = TimeSpan.FromHours(1),
        ["DISK_WARNING"]       = TimeSpan.FromHours(24),
        ["AI_CRITICAL"]        = TimeSpan.FromMinutes(30),
        ["ANTIVIRUS_DISABLED"] = TimeSpan.FromMinutes(30),
    };

    public async Task<bool> ShouldSendAsync(
        Guid tenantId, Guid machineId, string alertType, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(alertType);

        var key = $"alert_cooldown:{tenantId}:{machineId}:{alertType}";
        var ttl = Cooldowns.GetValueOrDefault(alertType, TimeSpan.FromHours(1));
        var db = redis.GetDatabase();
        return await db.StringSetAsync(key, "1", ttl, false, When.NotExists)
            .ConfigureAwait(false);
    }
}
