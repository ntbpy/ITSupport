using Mediator;
using MIT.Framework.Shared.Persistence;
using MIT.Modules.Diagnostics.Contracts.Dtos;

namespace MIT.Modules.Diagnostics.Contracts.v1.Diagnostics;

public sealed record ListDiagnosticsQuery(
    Guid MachineId,
    int Page = 1,
    int PageSize = 20) : IQuery<PagedResponse<DiagnosticReportDto>>;
