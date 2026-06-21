namespace MIT.Modules.Alerts.Contracts.Dtos;

public sealed record AlertDto(
    Guid Id,
    Guid MachineId,
    string AlertType,
    AlertSeverity Severity,
    string Message,
    string SentViaJson,
    DateTime SentAt,
    DateTime? AcknowledgedAt,
    Guid? AcknowledgedBy);
