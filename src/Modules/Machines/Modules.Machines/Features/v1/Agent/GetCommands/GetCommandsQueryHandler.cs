using MIT.Modules.Machines.Contracts.v1.Machines;
using MIT.Modules.Machines.Infrastructure;
using Mediator;

namespace MIT.Modules.Machines.Features.v1.Agent.GetCommands;

public sealed class GetCommandsQueryHandler(IMachineCommandQueueService commandQueue)
    : IQueryHandler<GetCommandsQuery, IReadOnlyList<PendingCommandDto>>
{
    public async ValueTask<IReadOnlyList<PendingCommandDto>> Handle(
        GetCommandsQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);
        var deadline = DateTime.UtcNow.AddSeconds(30);
        while (DateTime.UtcNow < deadline)
        {
            if (await commandQueue.HasCommandsAsync(query.MachineId, cancellationToken)
                .ConfigureAwait(false))
                return await commandQueue.DequeueAsync(query.MachineId, cancellationToken)
                    .ConfigureAwait(false);
            await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
        }
        return Array.Empty<PendingCommandDto>();
    }
}
