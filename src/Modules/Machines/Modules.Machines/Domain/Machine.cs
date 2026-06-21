using MIT.Framework.Core.Domain;
using MIT.Modules.Machines.Contracts.Dtos;

namespace MIT.Modules.Machines.Domain;

public sealed class Machine : AggregateRoot<Guid>
{
    public string MachineName { get; private set; } = default!;
    public string IpAddress { get; private set; } = default!;
    public string MacAddress { get; private set; } = default!;
    public string? OsVersion { get; private set; }
    public string? CpuModel { get; private set; }
    public decimal RamGb { get; private set; }
    public decimal DiskTotalGb { get; private set; }
    public string? AgentVersion { get; private set; }
    public DateTime? LastSeenAt { get; private set; }
    public MachineStatus Status { get; private set; }
    public string? AssignedUser { get; private set; }
    public Guid? GroupId { get; private set; }
    public string? Notes { get; private set; }
    public string? EncryptedApiKey { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Machine() { }

    public static Machine Create(
        string machineName, string ipAddress, string macAddress,
        string? osVersion, string? cpuModel, decimal ramGb, decimal diskTotalGb,
        string? agentVersion)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(machineName);
        ArgumentException.ThrowIfNullOrWhiteSpace(ipAddress);
        ArgumentException.ThrowIfNullOrWhiteSpace(macAddress);
        return new Machine
        {
            Id = Guid.CreateVersion7(),
            MachineName = machineName.Trim(),
            IpAddress = ipAddress.Trim(),
            MacAddress = macAddress.ToUpperInvariant().Trim(),
            OsVersion = osVersion,
            CpuModel = cpuModel,
            RamGb = ramGb,
            DiskTotalGb = diskTotalGb,
            AgentVersion = agentVersion,
            Status = MachineStatus.Offline,
            CreatedAt = DateTime.UtcNow,
        };
    }

    public void RecordHeartbeat()
    {
        Status = MachineStatus.Online;
        LastSeenAt = DateTime.UtcNow;
    }

    public void MarkOffline() => Status = MachineStatus.Offline;

    public void SetApiKey(string encryptedKey) => EncryptedApiKey = encryptedKey;

    public void UpdateInfo(string machineName, string ipAddress, string? osVersion,
        string? cpuModel, decimal ramGb, decimal diskTotalGb, string? agentVersion)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(machineName);
        ArgumentException.ThrowIfNullOrWhiteSpace(ipAddress);
        MachineName = machineName.Trim();
        IpAddress = ipAddress.Trim();
        OsVersion = osVersion;
        CpuModel = cpuModel;
        RamGb = ramGb;
        DiskTotalGb = diskTotalGb;
        AgentVersion = agentVersion;
    }

    public void UpdateNotes(string? notes) => Notes = notes?.Trim();
    public void AssignTo(string? user) => AssignedUser = user?.Trim();
    public void AssignGroup(Guid? groupId) => GroupId = groupId;
}
