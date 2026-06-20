using MIT.Framework.Core.Domain;
using MIT.Modules.Tickets.Contracts.Dtos;

namespace MIT.Modules.Tickets.Domain.Events;

public sealed record TicketStatusChangedDomainEvent(
    Guid TicketId,
    TicketStatus PreviousStatus,
    TicketStatus NewStatus,
    Guid EventId,
    DateTimeOffset OccurredOnUtc) : DomainEvent(EventId, OccurredOnUtc);
