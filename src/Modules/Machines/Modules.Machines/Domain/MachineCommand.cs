using MIT.Framework.Core.Domain;
using MIT.Modules.Machines.Contracts.Dtos;

namespace MIT.Modules.Machines.Domain;

public sealed class MachineCommand : AggregateRoot<Guid>
{
    public Guid MachineId { get; private set; }
    public CommandType Type { get; private set; }
    public string PayloadJson { get; private set; } = "{}";
    public CommandStatus Status { get; private set; }
    public string? OutputJson { get; private set; }
    public string? ErrorMessage { get; private set; }
    public Guid? RequestedByUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    private MachineCommand() { }

    public static MachineCommand Create(Guid machineId, CommandType type,
        string payloadJson, Guid? requestedByUserId = null) => new()
    {
        Id = Guid.CreateVersion7(),
        MachineId = machineId,
        Type = type,
        PayloadJson = payloadJson,
        Status = CommandStatus.Queued,
        RequestedByUserId = requestedByUserId,
        CreatedAt = DateTime.UtcNow,
    };

    public void MarkSent() => Status = CommandStatus.Sent;
    public void MarkReceived() => Status = CommandStatus.Received;

    public void Complete(string? outputJson, bool success, string? errorMessage = null)
    {
        Status = success ? CommandStatus.Completed : CommandStatus.Failed;
        OutputJson = outputJson;
        ErrorMessage = errorMessage;
        CompletedAt = DateTime.UtcNow;
    }
}
