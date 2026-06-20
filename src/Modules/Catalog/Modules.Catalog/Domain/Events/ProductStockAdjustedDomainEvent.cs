using MIT.Framework.Core.Domain;

namespace MIT.Modules.Catalog.Domain.Events;

public sealed record ProductStockAdjustedDomainEvent(
    Guid ProductId,
    int OldStock,
    int NewStock,
    int Delta,
    Guid EventId,
    DateTimeOffset OccurredOnUtc) : DomainEvent(EventId, OccurredOnUtc);
