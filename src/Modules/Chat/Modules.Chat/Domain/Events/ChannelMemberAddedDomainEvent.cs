using MIT.Framework.Core.Domain;

namespace MIT.Modules.Chat.Domain.Events;

public sealed record ChannelMemberAddedDomainEvent(
    Guid ChannelId,
    string AddedUserId,
    string AddedByUserId,
    Guid EventId,
    DateTimeOffset OccurredOnUtc) : DomainEvent(EventId, OccurredOnUtc);
