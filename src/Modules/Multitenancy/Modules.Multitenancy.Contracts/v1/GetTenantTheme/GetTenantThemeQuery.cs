using MIT.Modules.Multitenancy.Contracts.Dtos;
using Mediator;

namespace MIT.Modules.Multitenancy.Contracts.v1.GetTenantTheme;

public sealed record GetTenantThemeQuery : IQuery<TenantThemeDto>;