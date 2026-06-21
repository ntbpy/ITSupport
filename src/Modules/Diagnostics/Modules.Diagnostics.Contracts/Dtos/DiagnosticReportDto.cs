namespace MIT.Modules.Diagnostics.Contracts.Dtos;

public sealed record DiagnosticReportDto(
    Guid Id,
    Guid MachineId,
    DateTime AnalyzedAt,
    DiagnosticSeverity Severity,
    string IssuesJson,
    string FixesJson,
    string? AiSummary,
    bool AutoFixed,
    Guid? AcknowledgedBy,
    DateTime? AcknowledgedAt);
