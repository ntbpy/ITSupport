using Microsoft.AspNetCore.SignalR;

namespace MIT.Starter.Api.Hubs.Publishers;

internal interface ICommandPublisher
{
    Task NotifyCommandSent(Guid tenantId, Guid machineId, Guid commandId, string commandType, CancellationToken ct = default);
    Task NotifyCommandCompleted(Guid tenantId, Guid machineId, Guid commandId, bool success, CancellationToken ct = default);
}

internal sealed class CommandPublisher(IHubContext<CommandHub> hub) : ICommandPublisher
{
    public Task NotifyCommandSent(Guid tenantId, Guid machineId, Guid commandId,
        string commandType, CancellationToken ct = default)
        => hub.Clients.Group($"tenant_{tenantId}")
            .SendAsync("CommandSent", machineId, commandId, commandType, cancellationToken: ct);

    public Task NotifyCommandCompleted(Guid tenantId, Guid machineId, Guid commandId,
        bool success, CancellationToken ct = default)
        => hub.Clients.Group($"tenant_{tenantId}")
            .SendAsync("CommandCompleted", machineId, commandId, success, cancellationToken: ct);
}
