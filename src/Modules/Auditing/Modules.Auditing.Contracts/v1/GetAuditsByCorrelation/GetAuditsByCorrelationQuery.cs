using MIT.Modules.Auditing.Contracts.Dtos;
using Mediator;

namespace MIT.Modules.Auditing.Contracts.v1.GetAuditsByCorrelation;

public sealed class GetAuditsByCorrelationQuery : IQuery<IReadOnlyList<AuditSummaryDto>>
{
    public string CorrelationId { get; init; } = default!;

    public DateTime? FromUtc { get; init; }

    public DateTime? ToUtc { get; init; }
}