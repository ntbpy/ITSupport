using MIT.Modules.Machines.Contracts.v1.Machines;
using MIT.Modules.Machines.Data;
using MIT.Modules.Machines.Infrastructure;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace MIT.Modules.Machines.Features.v1.Agent.Heartbeat;

public sealed class HeartbeatCommandHandler(
    MachinesDbContext dbContext,
    IAgentMetricsPublisher metricsPublisher,
    IMachineCommandQueueService commandQueue)
    : ICommandHandler<HeartbeatCommand, HeartbeatResponse>
{
    public async ValueTask<HeartbeatResponse> Handle(
        HeartbeatCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var machine = await dbContext.Machines
            .FirstOrDefaultAsync(m => m.Id == command.MachineId, cancellationToken)
            .ConfigureAwait(false);

        if (machine is null) return new HeartbeatResponse(false, DateTime.UtcNow);

        machine.RecordHeartbeat();
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        await metricsPublisher.PublishHeartbeatAsync(command, cancellationToken).ConfigureAwait(false);

        var hasCommands = await commandQueue.HasCommandsAsync(command.MachineId, cancellationToken)
            .ConfigureAwait(false);
        return new HeartbeatResponse(hasCommands, DateTime.UtcNow);
    }
}
