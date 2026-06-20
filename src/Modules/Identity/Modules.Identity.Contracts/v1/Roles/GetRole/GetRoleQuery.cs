using MIT.Modules.Identity.Contracts.DTOs;
using Mediator;

namespace MIT.Modules.Identity.Contracts.v1.Roles.GetRole;

public sealed record GetRoleQuery(string Id) : IQuery<RoleDto?>;