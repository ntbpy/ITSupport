using MIT.Framework.Core.Domain;
using MIT.Modules.Diagnostics.Contracts.Dtos;

namespace MIT.Modules.Diagnostics.Domain;

public sealed class DiagnosticReport : AggregateRoot<Guid>
{
    public Guid MachineId { get; private set; }
    public DateTime AnalyzedAt { get; private set; }
    public DiagnosticSeverity Severity { get; private set; }
    public string IssuesJson { get; private set; } = "[]";
    public string FixesJson { get; private set; } = "[]";
    public string? AiSummary { get; private set; }
    public bool AutoFixed { get; private set; }
    public Guid? AcknowledgedBy { get; private set; }
    public DateTime? AcknowledgedAt { get; private set; }

    private DiagnosticReport() { }

    public static DiagnosticReport Create(
        Guid machineId, DiagnosticSeverity severity,
        string issuesJson, string fixesJson, string? summary)
    {
        return new DiagnosticReport
        {
            Id = Guid.CreateVersion7(),
            MachineId = machineId,
            AnalyzedAt = DateTime.UtcNow,
            Severity = severity,
            IssuesJson = issuesJson,
            FixesJson = fixesJson,
            AiSummary = summary,
        };
    }

    public void Acknowledge(Guid userId)
    {
        AcknowledgedBy = userId;
        AcknowledgedAt = DateTime.UtcNow;
    }

    public void MarkAutoFixed() => AutoFixed = true;
}
