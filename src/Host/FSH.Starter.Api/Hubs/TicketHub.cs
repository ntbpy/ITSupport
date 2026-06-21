using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MIT.Starter.Api.Hubs;

[Authorize]
internal sealed class TicketHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var tenantId = Context.User?.FindFirst("tenant_id")?.Value;
        if (tenantId is not null)
            await Groups.AddToGroupAsync(Context.ConnectionId, $"tenant_{tenantId}");
        await base.OnConnectedAsync();
    }
}
