using MIT.Framework.Shared.Quota;
using Mediator;

namespace MIT.Modules.Billing.Contracts.v1.Plans;

public sealed record CreatePlanCommand(
    string Key,
    string Name,
    string Currency,
    decimal MonthlyBasePrice,
    IReadOnlyDictionary<QuotaResource, decimal>? OverageRates = null,
    PlanInterval Interval = PlanInterval.Monthly,
    decimal? AnnualPrice = null) : ICommand<Guid>;
