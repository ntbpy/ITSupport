using Microsoft.AspNetCore.SignalR;

namespace MIT.Starter.Api.Hubs.Publishers;

internal interface IMachineStatusPublisher
{
    Task NotifyOnline(Guid tenantId, Guid machineId, CancellationToken ct = default);
    Task NotifyOffline(Guid tenantId, Guid machineId, CancellationToken ct = default);
    Task NotifyMetricsUpdate(Guid tenantId, Guid machineId, object metrics, CancellationToken ct = default);
    Task NotifyAlert(Guid tenantId, Guid machineId, string alertType, string severity, string message, CancellationToken ct = default);
}

internal sealed class MachineStatusPublisher(IHubContext<MachineStatusHub> hub) : IMachineStatusPublisher
{
    public Task NotifyOnline(Guid tenantId, Guid machineId, CancellationToken ct = default)
        => hub.Clients.Group($"tenant_{tenantId}")
            .SendAsync("MachineOnline", machineId, DateTime.UtcNow, cancellationToken: ct);

    public Task NotifyOffline(Guid tenantId, Guid machineId, CancellationToken ct = default)
        => hub.Clients.Group($"tenant_{tenantId}")
            .SendAsync("MachineOffline", machineId, DateTime.UtcNow, cancellationToken: ct);

    public Task NotifyMetricsUpdate(Guid tenantId, Guid machineId, object metrics, CancellationToken ct = default)
        => hub.Clients.Group($"tenant_{tenantId}")
            .SendAsync("MetricsUpdate", machineId, metrics, cancellationToken: ct);

    public Task NotifyAlert(Guid tenantId, Guid machineId, string alertType, string severity,
        string message, CancellationToken ct = default)
        => hub.Clients.Group($"tenant_{tenantId}")
            .SendAsync("AlertTriggered", machineId, alertType, severity, message, cancellationToken: ct);
}
