using MIT.Modules.Multitenancy.Contracts.Dtos;
using Mediator;

namespace MIT.Modules.Multitenancy.Contracts.v1.GetTenantStatus;

public sealed record GetTenantStatusQuery(string TenantId) : IQuery<TenantStatusDto>;