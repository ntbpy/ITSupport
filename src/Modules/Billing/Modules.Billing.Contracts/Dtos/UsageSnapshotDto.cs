using MIT.Framework.Shared.Quota;

namespace MIT.Modules.Billing.Contracts.Dtos;

public sealed record UsageSnapshotDto(
    Guid Id,
    string TenantId,
    int PeriodYear,
    int PeriodMonth,
    QuotaResource Resource,
    long UsedUnits,
    long LimitUnits,
    long Overage,
    DateTime CapturedAtUtc);
