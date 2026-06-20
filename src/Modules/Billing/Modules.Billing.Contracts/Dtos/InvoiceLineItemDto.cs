using MIT.Framework.Shared.Quota;

namespace MIT.Modules.Billing.Contracts.Dtos;

public sealed record InvoiceLineItemDto(
    Guid Id,
    InvoiceLineItemKind Kind,
    QuotaResource? Resource,
    string Description,
    decimal Quantity,
    decimal UnitPrice,
    decimal Amount);
