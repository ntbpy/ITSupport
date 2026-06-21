using MIT.Modules.Alerts.Contracts.Dtos;
using MIT.Modules.Alerts.Domain;

namespace Alerts.Tests.Domain;

public sealed class AlertTests
{
    [Fact]
    public void Create_SetsPropertiesCorrectly()
    {
        var machineId = Guid.NewGuid();
        var before = DateTime.UtcNow;

        var alert = Alert.Create(machineId, "CPU_HIGH", AlertSeverity.Warning,
            "CPU usage high", ["zalo", "email"]);

        alert.Id.ShouldNotBe(Guid.Empty);
        alert.MachineId.ShouldBe(machineId);
        alert.AlertType.ShouldBe("CPU_HIGH");
        alert.Severity.ShouldBe(AlertSeverity.Warning);
        alert.AcknowledgedAt.ShouldBeNull();
        alert.SentAt.ShouldBeGreaterThan(before);
    }

    [Fact]
    public void Acknowledge_SetsFields()
    {
        var alert = Alert.Create(Guid.NewGuid(), "MACHINE_OFFLINE",
            AlertSeverity.Critical, "Offline", []);
        var userId = Guid.NewGuid();

        alert.Acknowledge(userId);

        alert.AcknowledgedBy.ShouldBe(userId);
        alert.AcknowledgedAt.ShouldNotBeNull();
    }
}
