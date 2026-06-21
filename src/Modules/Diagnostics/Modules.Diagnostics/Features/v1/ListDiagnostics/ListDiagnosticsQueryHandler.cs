using MIT.Framework.Shared.Persistence;
using MIT.Modules.Diagnostics.Contracts.Dtos;
using MIT.Modules.Diagnostics.Contracts.v1.Diagnostics;
using MIT.Modules.Diagnostics.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace MIT.Modules.Diagnostics.Features.v1.ListDiagnostics;

public sealed class ListDiagnosticsQueryHandler(DiagnosticsDbContext dbContext)
    : IQueryHandler<ListDiagnosticsQuery, PagedResponse<DiagnosticReportDto>>
{
    public async ValueTask<PagedResponse<DiagnosticReportDto>> Handle(
        ListDiagnosticsQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var q = dbContext.DiagnosticReports
            .AsNoTracking()
            .Where(r => r.MachineId == query.MachineId)
            .OrderByDescending(r => r.AnalyzedAt);

        var total = await q.LongCountAsync(cancellationToken).ConfigureAwait(false);
        var items = await q
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(r => new DiagnosticReportDto(
                r.Id, r.MachineId, r.AnalyzedAt, r.Severity,
                r.IssuesJson, r.FixesJson, r.AiSummary,
                r.AutoFixed, r.AcknowledgedBy, r.AcknowledgedAt))
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        return new PagedResponse<DiagnosticReportDto>
        {
            Items = items,
            TotalCount = total,
            PageNumber = query.Page,
            PageSize = query.PageSize,
            TotalPages = (int)Math.Ceiling(total / (double)query.PageSize),
        };
    }
}
