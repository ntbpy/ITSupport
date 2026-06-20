using MIT.Modules.Billing.Contracts.Dtos;
using Mediator;

namespace MIT.Modules.Billing.Contracts.v1.Usage;

public sealed record GetUsageSnapshotsQuery(
    string? TenantId = null,
    int? PeriodYear = null,
    int? PeriodMonth = null) : IQuery<IReadOnlyList<UsageSnapshotDto>>;
