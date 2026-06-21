using MIT.Framework.Core.Context;
using MIT.Modules.Diagnostics.Contracts.v1.Diagnostics;
using MIT.Modules.Diagnostics.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace MIT.Modules.Diagnostics.Features.v1.AcknowledgeDiagnostic;

public sealed class AcknowledgeDiagnosticCommandHandler(
    DiagnosticsDbContext dbContext,
    ICurrentUser currentUser) : ICommandHandler<AcknowledgeDiagnosticCommand>
{
    public async ValueTask<Unit> Handle(
        AcknowledgeDiagnosticCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var report = await dbContext.DiagnosticReports
            .FirstOrDefaultAsync(r => r.Id == command.ReportId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new KeyNotFoundException($"DiagnosticReport {command.ReportId} not found");

        report.Acknowledge(currentUser.GetUserId());
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Unit.Value;
    }
}
