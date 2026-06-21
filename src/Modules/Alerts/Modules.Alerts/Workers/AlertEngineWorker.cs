using MIT.Modules.Alerts.Contracts.Dtos;
using MIT.Modules.Alerts.Data;
using MIT.Modules.Alerts.Domain;
using MIT.Modules.Alerts.Infrastructure;
using MIT.Modules.Machines.Contracts.Dtos;
using MIT.Modules.Machines.Contracts.v1.Machines;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MIT.Modules.Alerts.Workers;

public sealed partial class AlertEngineWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<AlertEngineWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckAllTenantsAsync(stoppingToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LogTickError(logger, ex);
            }
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken).ConfigureAwait(false);
        }
    }

    private async Task CheckAllTenantsAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var alertsDb = scope.ServiceProvider.GetRequiredService<AlertsDbContext>();
        var dedup = scope.ServiceProvider.GetRequiredService<AlertDeduplicationService>();
        var notifier = scope.ServiceProvider.GetRequiredService<AlertNotificationOrchestrator>();

        // Check for machines that went offline (last_seen_at > 10 min ago, status still Online).
        var offlineThreshold = DateTime.UtcNow.AddMinutes(-10);
        var machines = await mediator.Send(
            new ListMachinesQuery(Page: 1, PageSize: 200, Status: MachineStatus.Online), ct)
            .ConfigureAwait(false);

        var offlineMachines = machines.Items
            .Where(m => m.LastSeenAt.HasValue && m.LastSeenAt.Value < offlineThreshold)
            .ToList();

        foreach (var machine in offlineMachines)
        {
            // Use Guid.Empty as tenant placeholder until Finbuckle context flows into scope.
            if (!await dedup.ShouldSendAsync(Guid.Empty, machine.Id, "MACHINE_OFFLINE", ct)
                .ConfigureAwait(false)) continue;

            var alert = Alert.Create(machine.Id, "MACHINE_OFFLINE",
                AlertSeverity.Critical,
                $"Machine {machine.MachineName} has gone offline",
                ["zalo", "email"]);
            alertsDb.Alerts.Add(alert);

            await notifier.NotifyOfflineAsync(machine.MachineName, ct).ConfigureAwait(false);
        }

        await alertsDb.SaveChangesAsync(ct).ConfigureAwait(false);
        LogTick(logger, offlineMachines.Count);
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "AlertEngine tick failed")]
    private static partial void LogTickError(ILogger logger, Exception ex);

    [LoggerMessage(Level = LogLevel.Debug,
        Message = "AlertEngine tick: {OfflineCount} potential offline machines processed")]
    private static partial void LogTick(ILogger logger, int offlineCount);
}
