using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace MIT.Modules.Alerts.Infrastructure;

public sealed partial class ZaloOAService(
    HttpClient httpClient,
    ILogger<ZaloOAService> logger)
{
    public async Task SendAsync(
        string accessToken, string recipientId, string message, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accessToken);
        ArgumentException.ThrowIfNullOrWhiteSpace(recipientId);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        var payload = new
        {
            recipient = new { user_id = recipientId },
            message = new { text = message }
        };
        httpClient.DefaultRequestHeaders.Remove("access_token");
        httpClient.DefaultRequestHeaders.Add("access_token", accessToken);
        using var response = await httpClient.PostAsJsonAsync(
            "v2.0/oa/message", payload, ct).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
            LogSendFailed(logger, response.StatusCode);
    }

    public static string FormatOfflineMessage(
        string machineName, string companyName, DateTime time, string dashboardUrl)
        => $"\U0001f534 [VietRMM] Máy {machineName} tại {companyName} đã mất kết nối lúc {time:HH:mm dd/MM}. Xem: {dashboardUrl}";

    public static string FormatCpuMessage(string machineName, decimal cpuPct, int durationMin)
        => $"⚠️ [VietRMM] Cảnh báo CPU cao: Máy {machineName} đang dùng {cpuPct:F0}% CPU trong {durationMin} phút.";

    public static string FormatCriticalMessage(string machineName, string summary)
        => $"\U0001f6a8 [VietRMM] Phát hiện sự cố nghiêm trọng trên máy {machineName}: {summary}";

    [LoggerMessage(Level = LogLevel.Warning, Message = "Zalo OA send failed: {StatusCode}")]
    private static partial void LogSendFailed(
        ILogger logger, System.Net.HttpStatusCode statusCode);
}
