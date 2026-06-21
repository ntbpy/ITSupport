using MIT.Framework.Shared.Persistence;
using MIT.Modules.Alerts.Contracts.Dtos;
using MIT.Modules.Alerts.Contracts.v1.Alerts;
using MIT.Modules.Alerts.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace MIT.Modules.Alerts.Features.v1.ListAlerts;

public sealed class ListAlertsQueryHandler(AlertsDbContext dbContext)
    : IQueryHandler<ListAlertsQuery, PagedResponse<AlertDto>>
{
    public async ValueTask<PagedResponse<AlertDto>> Handle(
        ListAlertsQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var q = dbContext.Alerts.AsNoTracking();
        if (query.MachineId.HasValue)
            q = q.Where(a => a.MachineId == query.MachineId.Value);
        if (query.UnacknowledgedOnly)
            q = q.Where(a => a.AcknowledgedAt == null);

        q = q.OrderByDescending(a => a.SentAt);

        var total = await q.LongCountAsync(cancellationToken).ConfigureAwait(false);
        var items = await q
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(a => new AlertDto(
                a.Id, a.MachineId, a.AlertType, a.Severity, a.Message,
                a.SentViaJson, a.SentAt, a.AcknowledgedAt, a.AcknowledgedBy))
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        return new PagedResponse<AlertDto>
        {
            Items = items,
            TotalCount = total,
            PageNumber = query.Page,
            PageSize = query.PageSize,
            TotalPages = (int)Math.Ceiling(total / (double)query.PageSize),
        };
    }
}
