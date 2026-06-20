using MIT.Modules.Tickets.Contracts.Dtos;
using Mediator;

namespace MIT.Modules.Tickets.Contracts.v1.Tickets;

public sealed record ListTicketCommentsQuery(Guid TicketId) : IQuery<IReadOnlyList<TicketCommentDto>>;
