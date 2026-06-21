using MIT.Framework.Core.Domain;

namespace MIT.Modules.Machines.Domain;

public sealed class PatchStatus : BaseEntity<Guid>
{
    public Guid MachineId { get; private set; }
    public string PendingUpdatesJson { get; private set; } = "[]";
    public DateTime LastCheckedAt { get; private set; }

    private PatchStatus() { }

    public static PatchStatus Create(Guid machineId) => new()
    {
        Id = Guid.CreateVersion7(),
        MachineId = machineId,
        LastCheckedAt = DateTime.UtcNow,
    };

    public void Update(string pendingUpdatesJson)
    {
        PendingUpdatesJson = pendingUpdatesJson;
        LastCheckedAt = DateTime.UtcNow;
    }
}
