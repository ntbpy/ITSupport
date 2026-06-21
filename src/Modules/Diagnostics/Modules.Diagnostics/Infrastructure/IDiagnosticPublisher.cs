using MIT.Modules.Diagnostics.Contracts.Dtos;

namespace MIT.Modules.Diagnostics.Infrastructure;

public interface IDiagnosticPublisher
{
    Task NotifyStarted(Guid machineId, CancellationToken ct = default);
    Task NotifyCompleted(Guid machineId, DiagnosticSeverity severity,
        int issueCount, string summary, CancellationToken ct = default);
}

// No-op publisher; replaced in Task 10 when SignalR hubs are wired up.
public sealed class NoOpDiagnosticPublisher : IDiagnosticPublisher
{
    public Task NotifyStarted(Guid machineId, CancellationToken ct = default) => Task.CompletedTask;
    public Task NotifyCompleted(Guid machineId, DiagnosticSeverity severity,
        int issueCount, string summary, CancellationToken ct = default) => Task.CompletedTask;
}
