using MIT.Modules.Machines.Contracts.v1.Machines;
using MIT.Modules.Machines.Data;
using MIT.Modules.Machines.Domain;
using MIT.Modules.Machines.Infrastructure;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace MIT.Modules.Machines.Features.v1.Agent.PostSnapshot;

public sealed class PostSnapshotCommandHandler(
    MachinesDbContext dbContext,
    IAgentMetricsPublisher metricsPublisher)
    : ICommandHandler<PostSnapshotCommand>
{
    public async ValueTask<Unit> Handle(PostSnapshotCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var patchStatus = await dbContext.PatchStatuses
            .FirstOrDefaultAsync(p => p.MachineId == command.MachineId, cancellationToken)
            .ConfigureAwait(false);

        if (patchStatus is null)
        {
            patchStatus = PatchStatus.Create(command.MachineId);
            dbContext.PatchStatuses.Add(patchStatus);
        }

        patchStatus.Update(command.WindowsUpdatePendingJson);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        await metricsPublisher.PublishSnapshotAsync(command, cancellationToken).ConfigureAwait(false);
        return Unit.Value;
    }
}
