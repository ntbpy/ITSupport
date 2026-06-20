using MIT.Framework.Core.Domain;

namespace MIT.Modules.Tickets.Domain.Events;

public sealed record TicketAssignedDomainEvent(
    Guid TicketId,
    Guid? PreviousAssigneeUserId,
    Guid? NewAssigneeUserId,
    Guid EventId,
    DateTimeOffset OccurredOnUtc) : DomainEvent(EventId, OccurredOnUtc);
