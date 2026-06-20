using MIT.Modules.Multitenancy.Contracts.Dtos;
using Mediator;

namespace MIT.Modules.Multitenancy.Contracts.v1.TenantProvisioning;

public sealed record RetryTenantProvisioningCommand(string TenantId) : ICommand<TenantProvisioningStatusDto>;