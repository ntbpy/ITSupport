using MIT.Framework.Core.Domain;

namespace MIT.Modules.Chat.Domain.Events;

public sealed record MessagePinnedDomainEvent(
    Guid ChannelId,
    Guid MessageId,
    string PinnedByUserId,
    Guid EventId,
    DateTimeOffset OccurredOnUtc) : DomainEvent(EventId, OccurredOnUtc);
