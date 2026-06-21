using MIT.Framework.Core.Context;
using MIT.Modules.Alerts.Contracts.v1.Alerts;
using MIT.Modules.Alerts.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace MIT.Modules.Alerts.Features.v1.AcknowledgeAlert;

public sealed class AcknowledgeAlertCommandHandler(
    AlertsDbContext dbContext,
    ICurrentUser currentUser) : ICommandHandler<AcknowledgeAlertCommand>
{
    public async ValueTask<Unit> Handle(
        AcknowledgeAlertCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var alert = await dbContext.Alerts
            .FirstOrDefaultAsync(a => a.Id == command.AlertId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new KeyNotFoundException($"Alert {command.AlertId} not found");

        alert.Acknowledge(currentUser.GetUserId());
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Unit.Value;
    }
}
