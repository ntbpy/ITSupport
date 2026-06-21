using Mediator;
using MIT.Framework.Shared.Persistence;
using MIT.Modules.Alerts.Contracts.Dtos;

namespace MIT.Modules.Alerts.Contracts.v1.Alerts;

public sealed record ListAlertsQuery(
    Guid? MachineId = null,
    bool UnacknowledgedOnly = false,
    int Page = 1,
    int PageSize = 20) : IQuery<PagedResponse<AlertDto>>;
