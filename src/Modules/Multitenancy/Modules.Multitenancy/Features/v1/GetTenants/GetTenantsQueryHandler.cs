using MIT.Framework.Shared.Persistence;
using MIT.Modules.Multitenancy.Contracts;
using MIT.Modules.Multitenancy.Contracts.Dtos;
using MIT.Modules.Multitenancy.Contracts.v1.GetTenants;
using Mediator;

namespace MIT.Modules.Multitenancy.Features.v1.GetTenants;

public sealed class GetTenantsQueryHandler(ITenantService tenantService)
    : IQueryHandler<GetTenantsQuery, PagedResponse<TenantDto>>
{
    public async ValueTask<PagedResponse<TenantDto>> Handle(GetTenantsQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);
        return await tenantService.GetAllAsync(query, cancellationToken).ConfigureAwait(false);
    }
}