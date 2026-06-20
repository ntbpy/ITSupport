using MIT.Modules.Identity.Contracts.DTOs;
using Mediator;

namespace MIT.Modules.Identity.Contracts.v1.Groups.GetGroupMembers;

public sealed record GetGroupMembersQuery(Guid GroupId) : IQuery<IEnumerable<GroupMemberDto>>;