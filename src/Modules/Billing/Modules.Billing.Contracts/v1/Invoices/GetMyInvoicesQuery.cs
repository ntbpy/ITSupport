using MIT.Framework.Shared.Persistence;
using MIT.Modules.Billing.Contracts.Dtos;
using Mediator;

namespace MIT.Modules.Billing.Contracts.v1.Invoices;

public sealed record GetMyInvoicesQuery(
    InvoiceStatus? Status = null,
    int? PeriodYear = null,
    int? PeriodMonth = null,
    int PageNumber = 1,
    int PageSize = 20) : IQuery<PagedResponse<InvoiceDto>>;
