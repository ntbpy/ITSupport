using MIT.Framework.Core.Domain;
using MIT.Modules.Alerts.Contracts.Dtos;
using System.Text.Json;

namespace MIT.Modules.Alerts.Domain;

public sealed class Alert : AggregateRoot<Guid>
{
    public Guid MachineId { get; private set; }
    public string AlertType { get; private set; } = default!;
    public AlertSeverity Severity { get; private set; }
    public string Message { get; private set; } = default!;
    public string SentViaJson { get; private set; } = "[]";
    public DateTime SentAt { get; private set; }
    public DateTime? AcknowledgedAt { get; private set; }
    public Guid? AcknowledgedBy { get; private set; }

    private Alert() { }

    public static Alert Create(
        Guid machineId, string alertType,
        AlertSeverity severity, string message, string[] channels)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(alertType);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        return new Alert
        {
            Id = Guid.CreateVersion7(),
            MachineId = machineId,
            AlertType = alertType,
            Severity = severity,
            Message = message,
            SentViaJson = JsonSerializer.Serialize(channels),
            SentAt = DateTime.UtcNow,
        };
    }

    public void Acknowledge(Guid userId)
    {
        AcknowledgedBy = userId;
        AcknowledgedAt = DateTime.UtcNow;
    }
}
