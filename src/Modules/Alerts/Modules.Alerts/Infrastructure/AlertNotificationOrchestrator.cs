using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MIT.Modules.Alerts.Infrastructure;

public sealed partial class AlertNotificationOrchestrator(
    ZaloOAService zaloService,
    IOptions<ZaloOAOptions> options,
    ILogger<AlertNotificationOrchestrator> logger)
{
    public async Task NotifyOfflineAsync(string machineName, CancellationToken ct = default)
    {
        var msg = ZaloOAService.FormatOfflineMessage(
            machineName, "Company", DateTime.UtcNow, options.Value.DashboardUrl);
        await SendZaloAsync(msg, ct).ConfigureAwait(false);
    }

    public async Task NotifyCriticalDiagnosticAsync(
        string machineName, string summary, CancellationToken ct = default)
    {
        var msg = ZaloOAService.FormatCriticalMessage(machineName, summary);
        await SendZaloAsync(msg, ct).ConfigureAwait(false);
    }

    private async Task SendZaloAsync(string message, CancellationToken ct)
    {
        try
        {
            await zaloService.SendAsync(
                options.Value.AccessToken, options.Value.RecipientId, message, ct)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            LogZaloError(logger, ex);
        }
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Zalo OA notification failed")]
    private static partial void LogZaloError(ILogger logger, Exception ex);
}
