using MIT.Modules.Billing.Contracts.Dtos;

namespace MIT.Modules.Billing.Services;

/// <summary>Renders an invoice to a self-contained PDF document (on-demand, no stored artifact).</summary>
public interface IInvoicePdfRenderer
{
    byte[] Render(InvoiceDto invoice);
}
