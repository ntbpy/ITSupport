using MIT.Modules.Identity.Contracts.DTOs;
using Mediator;

namespace MIT.Modules.Identity.Contracts.v1.Groups.GetGroups;

public sealed record GetGroupsQuery(string? SearchTerm = null) : IQuery<IEnumerable<GroupDto>>;