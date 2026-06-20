using MIT.Framework.Core.Exceptions;
using MIT.Modules.Tickets.Contracts.Dtos;
using MIT.Modules.Tickets.Contracts.v1.Tickets;
using MIT.Modules.Tickets.Data;
using MIT.Modules.Tickets.Domain;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace MIT.Modules.Tickets.Features.v1.Tickets.GetTicketById;

public sealed class GetTicketByIdQueryHandler(TicketsDbContext dbContext)
    : IQueryHandler<GetTicketByIdQuery, TicketDto>
{
    public async ValueTask<TicketDto> Handle(GetTicketByIdQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var ticket = await dbContext.Tickets
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == query.TicketId, cancellationToken)
            .ConfigureAwait(false);

        if (ticket is null)
        {
            throw new NotFoundException($"Ticket {query.TicketId} not found.");
        }

        int commentCount = await dbContext.TicketComments
            .CountAsync(c => c.TicketId == ticket.Id, cancellationToken)
            .ConfigureAwait(false);

        return ticket.ToDto(commentCount);
    }
}
