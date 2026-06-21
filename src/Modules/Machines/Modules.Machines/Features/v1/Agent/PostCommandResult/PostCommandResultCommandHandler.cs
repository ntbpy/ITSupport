using MIT.Modules.Machines.Contracts.v1.Machines;
using MIT.Modules.Machines.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace MIT.Modules.Machines.Features.v1.Agent.PostCommandResult;

public sealed class PostCommandResultCommandHandler(MachinesDbContext dbContext)
    : ICommandHandler<PostCommandResultCommand>
{
    public async ValueTask<Unit> Handle(
        PostCommandResultCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        var cmd = await dbContext.MachineCommands
            .FirstOrDefaultAsync(c => c.Id == command.CommandId, cancellationToken)
            .ConfigureAwait(false);
        if (cmd is null) return Unit.Value;
        cmd.Complete(command.OutputJson, command.Success, command.ErrorMessage);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Unit.Value;
    }
}
