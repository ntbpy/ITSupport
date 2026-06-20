using MIT.Modules.Billing.Contracts.Dtos;
using Mediator;

namespace MIT.Modules.Billing.Contracts.v1.Invoices;

public sealed record GetInvoiceByIdQuery(Guid InvoiceId) : IQuery<InvoiceDto>;
