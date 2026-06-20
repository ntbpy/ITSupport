using MIT.Modules.Multitenancy.Contracts.Dtos;
using Mediator;

namespace MIT.Modules.Multitenancy.Contracts.v1.GetTenantMigrations;

public sealed record GetTenantMigrationsQuery : IQuery<IReadOnlyCollection<TenantMigrationStatusDto>>;