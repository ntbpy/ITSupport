using MIT.Framework.Core.Context;
using MIT.Modules.Machines.Contracts.v1.Machines;
using MIT.Modules.Machines.Data;
using MIT.Modules.Machines.Domain;
using MIT.Modules.Machines.Infrastructure;
using Mediator;
using System.Text.Json;

namespace MIT.Modules.Machines.Features.v1.Admin.SendMachineCommand;

public sealed class SendMachineCommandHandler(
    MachinesDbContext dbContext,
    IMachineCommandQueueService commandQueue,
    ICurrentUser currentUser)
    : ICommandHandler<SendMachineCommandCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        SendMachineCommandCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var cmd = MachineCommand.Create(
            command.MachineId, command.Type, command.PayloadJson,
            currentUser.GetUserId());
        dbContext.MachineCommands.Add(cmd);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        var envelope = JsonSerializer.Serialize(new
        {
            commandId = cmd.Id,
            type = cmd.Type.ToString(),
            payload = cmd.PayloadJson,
        });
        await commandQueue.EnqueueAsync(command.MachineId, envelope, cancellationToken)
            .ConfigureAwait(false);
        return cmd.Id;
    }
}
