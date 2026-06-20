using MIT.Modules.Billing.Contracts.Dtos;
using MIT.Modules.Billing.Domain;

namespace MIT.Modules.Billing.Features.v1.Invoices;

internal static class InvoiceMappings
{
    public static InvoiceDto ToDto(this Invoice invoice) => new(
        invoice.Id,
        invoice.TenantId,
        invoice.InvoiceNumber,
        invoice.PeriodYear,
        invoice.PeriodMonth,
        invoice.Currency,
        invoice.SubtotalAmount,
        invoice.Status,
        invoice.CreatedAtUtc,
        invoice.IssuedAtUtc,
        invoice.DueAtUtc,
        invoice.PaidAtUtc,
        invoice.VoidedAtUtc,
        invoice.Notes,
        invoice.LineItems
            .Select(l => new InvoiceLineItemDto(l.Id, l.Kind, l.Resource, l.Description, l.Quantity, l.UnitPrice, l.Amount))
            .ToList(),
        invoice.Purpose,
        invoice.PeriodStartUtc,
        invoice.PeriodEndUtc);
}
