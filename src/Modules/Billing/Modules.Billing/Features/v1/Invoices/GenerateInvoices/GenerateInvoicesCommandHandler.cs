using Finbuckle.MultiTenant.Abstractions;
using MIT.Framework.Core.Exceptions;
using MIT.Framework.Shared.Multitenancy;
using MIT.Modules.Billing.Contracts.v1.Invoices;
using MIT.Modules.Billing.Services;
using Mediator;

namespace MIT.Modules.Billing.Features.v1.Invoices.GenerateInvoices;

public sealed class GenerateInvoicesCommandHandler(
    IBillingService billing,
    IMultiTenantContextAccessor<AppTenantInfo> tenantAccessor)
    : ICommandHandler<GenerateInvoicesCommand, int>
{
    public async ValueTask<int> Handle(GenerateInvoicesCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        // Platform-wide invoice generation runs across EVERY tenant — it is a root-operator action.
        // A tenant admin (who also holds Billing.Manage) must not be able to trigger it.
        var callerTenantId = tenantAccessor.MultiTenantContext?.TenantInfo?.Id
            ?? throw new UnauthorizedException("Tenant context is required.");
        if (callerTenantId != MultitenancyConstants.Root.Id)
        {
            throw new ForbiddenException("Only the root operator may generate invoices across tenants.");
        }

        return await billing.GenerateInvoicesForAllTenantsAsync(command.PeriodYear, command.PeriodMonth, cancellationToken).ConfigureAwait(false);
    }
}
