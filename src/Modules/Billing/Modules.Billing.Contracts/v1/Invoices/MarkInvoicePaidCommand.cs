using Mediator;

namespace MIT.Modules.Billing.Contracts.v1.Invoices;

public sealed record MarkInvoicePaidCommand(Guid InvoiceId) : ICommand<Guid>;
