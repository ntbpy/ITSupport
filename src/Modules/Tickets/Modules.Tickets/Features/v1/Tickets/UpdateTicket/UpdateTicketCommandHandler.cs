using MIT.Framework.Core.Exceptions;
using MIT.Modules.Tickets.Contracts.v1.Tickets;
using MIT.Modules.Tickets.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace MIT.Modules.Tickets.Features.v1.Tickets.UpdateTicket;

public sealed class UpdateTicketCommandHandler(TicketsDbContext dbContext)
    : ICommandHandler<UpdateTicketCommand, Guid>
{
    public async ValueTask<Guid> Handle(UpdateTicketCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var ticket = await dbContext.Tickets
            .FirstOrDefaultAsync(t => t.Id == command.TicketId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException($"Ticket {command.TicketId} not found.");

        ticket.UpdateDetails(command.Title, command.Description, command.Priority);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return ticket.Id;
    }
}
