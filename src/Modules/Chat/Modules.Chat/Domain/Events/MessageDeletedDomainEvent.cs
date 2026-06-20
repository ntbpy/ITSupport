using MIT.Framework.Core.Domain;

namespace MIT.Modules.Chat.Domain.Events;

public sealed record MessageDeletedDomainEvent(
    Guid ChannelId,
    Guid MessageId,
    string AuthorUserId,
    Guid EventId,
    DateTimeOffset OccurredOnUtc) : DomainEvent(EventId, OccurredOnUtc);
