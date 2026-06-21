using MIT.Modules.Diagnostics.Contracts.Dtos;
using MIT.Modules.Diagnostics.Infrastructure;
using Microsoft.AspNetCore.SignalR;

namespace MIT.Starter.Api.Hubs.Publishers;

internal sealed class DiagnosticPublisher(IHubContext<DiagnosticHub> hub) : IDiagnosticPublisher
{
    public Task NotifyStarted(Guid machineId, CancellationToken ct = default)
        => hub.Clients.All.SendAsync("DiagnosticStarted", machineId, cancellationToken: ct);

    public Task NotifyCompleted(Guid machineId, DiagnosticSeverity severity,
        int issueCount, string summary, CancellationToken ct = default)
        => hub.Clients.All.SendAsync(
            "DiagnosticCompleted", machineId, severity.ToString(), issueCount, summary,
            cancellationToken: ct);
}
