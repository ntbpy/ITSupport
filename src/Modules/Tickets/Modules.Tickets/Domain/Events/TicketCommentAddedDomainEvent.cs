using MIT.Framework.Core.Domain;

namespace MIT.Modules.Tickets.Domain.Events;

public sealed record TicketCommentAddedDomainEvent(
    Guid TicketId,
    Guid CommentId,
    Guid AuthorUserId,
    Guid EventId,
    DateTimeOffset OccurredOnUtc) : DomainEvent(EventId, OccurredOnUtc);
