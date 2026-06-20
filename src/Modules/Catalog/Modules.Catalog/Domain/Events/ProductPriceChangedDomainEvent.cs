using MIT.Framework.Core.Domain;

namespace MIT.Modules.Catalog.Domain.Events;

public sealed record ProductPriceChangedDomainEvent(
    Guid ProductId,
    decimal OldAmount,
    decimal NewAmount,
    string Currency,
    Guid EventId,
    DateTimeOffset OccurredOnUtc) : DomainEvent(EventId, OccurredOnUtc);
