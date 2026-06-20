using MIT.Modules.Auditing.Contracts.Dtos;
using Mediator;

namespace MIT.Modules.Auditing.Contracts.v1.GetAuditById;

public sealed record GetAuditByIdQuery(Guid Id) : IQuery<AuditDetailDto>;