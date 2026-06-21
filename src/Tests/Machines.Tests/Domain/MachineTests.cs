using MIT.Modules.Machines.Contracts.Dtos;
using MIT.Modules.Machines.Domain;

namespace Machines.Tests.Domain;

public sealed class MachineTests
{
    [Fact]
    public void Create_ValidArgs_SetsOfflineStatus()
    {
        var machine = Machine.Create("PC-001", "192.168.1.1", "AA:BB:CC:DD:EE:FF",
            "Windows 11", "Intel i7", 16m, 512m, "1.0.0");
        machine.Status.ShouldBe(MachineStatus.Offline);
        machine.MachineName.ShouldBe("PC-001");
    }

    [Fact]
    public void RecordHeartbeat_SetsOnlineAndUpdatesLastSeen()
    {
        var machine = Machine.Create("PC-001", "192.168.1.1", "AA:BB:CC:DD:EE:FF",
            "Windows 11", "Intel i7", 16m, 512m, "1.0.0");
        var before = DateTime.UtcNow.AddSeconds(-1);
        machine.RecordHeartbeat();
        machine.Status.ShouldBe(MachineStatus.Online);
        machine.LastSeenAt.ShouldNotBeNull();
        machine.LastSeenAt.Value.ShouldBeGreaterThan(before);
    }

    [Fact]
    public void MarkOffline_SetsOfflineStatus()
    {
        var machine = Machine.Create("PC-001", "192.168.1.1", "AA:BB:CC:DD:EE:FF",
            "Windows 11", "Intel i7", 16m, 512m, "1.0.0");
        machine.RecordHeartbeat();
        machine.MarkOffline();
        machine.Status.ShouldBe(MachineStatus.Offline);
    }
}
