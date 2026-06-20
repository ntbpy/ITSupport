using Mediator;

namespace MIT.Modules.Tickets.Contracts.v1.Tickets;

public sealed record DeleteTicketCommand(Guid TicketId) : ICommand<Unit>;
