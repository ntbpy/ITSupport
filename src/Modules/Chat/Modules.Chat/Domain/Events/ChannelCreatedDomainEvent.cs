using MIT.Framework.Core.Domain;
using MIT.Modules.Chat.Contracts.v1.DTOs;

namespace MIT.Modules.Chat.Domain.Events;

public sealed record ChannelCreatedDomainEvent(
    Guid ChannelId,
    ChannelType Type,
    string? Name,
    string CreatedByUserId,
    Guid EventId,
    DateTimeOffset OccurredOnUtc) : DomainEvent(EventId, OccurredOnUtc);
