using MIT.Framework.Core.Domain;

namespace MIT.Modules.Machines.Domain;

public sealed class MachineGroup : AggregateRoot<Guid>
{
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public string AlertPolicyJson { get; private set; } = "{}";
    public DateTime CreatedAt { get; private set; }

    private MachineGroup() { }

    public static MachineGroup Create(string name, string? description = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return new()
        {
            Id = Guid.CreateVersion7(),
            Name = name.Trim(),
            Description = description?.Trim(),
            CreatedAt = DateTime.UtcNow,
        };
    }

    public void UpdateAlertPolicy(string policyJson) =>
        AlertPolicyJson = string.IsNullOrWhiteSpace(policyJson) ? "{}" : policyJson;
}
