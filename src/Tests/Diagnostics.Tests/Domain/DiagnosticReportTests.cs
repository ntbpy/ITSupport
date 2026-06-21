using MIT.Modules.Diagnostics.Contracts.Dtos;
using MIT.Modules.Diagnostics.Domain;

namespace Diagnostics.Tests.Domain;

public sealed class DiagnosticReportTests
{
    [Fact]
    public void Create_SetsPropertiesCorrectly()
    {
        var machineId = Guid.NewGuid();
        var before = DateTime.UtcNow;

        var report = DiagnosticReport.Create(
            machineId, DiagnosticSeverity.High,
            "[{\"title\":\"issue\"}]", "[{\"title\":\"fix\"}]", "summary");

        report.Id.ShouldNotBe(Guid.Empty);
        report.MachineId.ShouldBe(machineId);
        report.Severity.ShouldBe(DiagnosticSeverity.High);
        report.AutoFixed.ShouldBeFalse();
        report.AiSummary.ShouldBe("summary");
        report.AnalyzedAt.ShouldBeGreaterThan(before);
    }

    [Fact]
    public void Acknowledge_SetsAcknowledgedByAndAt()
    {
        var report = DiagnosticReport.Create(
            Guid.NewGuid(), DiagnosticSeverity.Low, "[]", "[]", null);
        var userId = Guid.NewGuid();

        report.Acknowledge(userId);

        report.AcknowledgedBy.ShouldBe(userId);
        report.AcknowledgedAt.ShouldNotBeNull();
    }

    [Fact]
    public void MarkAutoFixed_SetsAutoFixed()
    {
        var report = DiagnosticReport.Create(
            Guid.NewGuid(), DiagnosticSeverity.Critical, "[]", "[]", null);

        report.MarkAutoFixed();

        report.AutoFixed.ShouldBeTrue();
    }
}
