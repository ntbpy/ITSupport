using MIT.Modules.Billing.Contracts.v1.Invoices;
using MIT.Modules.Billing.Services;
using Mediator;

namespace MIT.Modules.Billing.Features.v1.Invoices.VoidInvoice;

public sealed class VoidInvoiceCommandHandler(IBillingService billing)
    : ICommandHandler<VoidInvoiceCommand, Guid>
{
    public async ValueTask<Guid> Handle(VoidInvoiceCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        await billing.VoidInvoiceAsync(command.InvoiceId, command.Reason, cancellationToken).ConfigureAwait(false);
        return command.InvoiceId;
    }
}
