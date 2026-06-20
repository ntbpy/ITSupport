using MIT.Modules.Identity.Contracts.DTOs;
using Mediator;

namespace MIT.Modules.Identity.Contracts.v1.Sessions.GetMySessions;

public sealed record GetMySessionsQuery : IQuery<List<UserSessionDto>>;