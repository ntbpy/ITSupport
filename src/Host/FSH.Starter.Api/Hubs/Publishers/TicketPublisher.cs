using Microsoft.AspNetCore.SignalR;

namespace MIT.Starter.Api.Hubs.Publishers;

internal interface ITicketPublisher
{
    Task NotifyTicketCreated(Guid tenantId, Guid ticketId, string title, CancellationToken ct = default);
    Task NotifyStatusChanged(Guid tenantId, Guid ticketId, string status, CancellationToken ct = default);
}

internal sealed class TicketPublisher(IHubContext<TicketHub> hub) : ITicketPublisher
{
    public Task NotifyTicketCreated(Guid tenantId, Guid ticketId, string title, CancellationToken ct = default)
        => hub.Clients.Group($"tenant_{tenantId}")
            .SendAsync("TicketCreated", ticketId, title, cancellationToken: ct);

    public Task NotifyStatusChanged(Guid tenantId, Guid ticketId, string status, CancellationToken ct = default)
        => hub.Clients.Group($"tenant_{tenantId}")
            .SendAsync("StatusChanged", ticketId, status, cancellationToken: ct);
}
