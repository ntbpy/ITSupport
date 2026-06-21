using MIT.Modules.Diagnostics.Contracts.v1.Diagnostics;
using MIT.Modules.Diagnostics.Data;
using MIT.Modules.Diagnostics.Domain;
using MIT.Modules.Diagnostics.Infrastructure;
using MIT.Modules.Machines.Contracts.v1.Machines;
using Mediator;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace MIT.Modules.Diagnostics.Features.v1.TriggerDiagnostic;

public sealed partial class TriggerDiagnosticCommandHandler(
    IMediator mediator,
    DiagnosticsDbContext diagnosticsDb,
    IClaudeApiService claudeApi,
    IDiagnosticPublisher publisher,
    ILogger<TriggerDiagnosticCommandHandler> logger)
    : ICommandHandler<TriggerDiagnosticCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        TriggerDiagnosticCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var machine = await mediator.Send(new GetMachineDetailQuery(command.MachineId), cancellationToken)
            .ConfigureAwait(false)
            ?? throw new KeyNotFoundException($"Machine {command.MachineId} not found");

        await publisher.NotifyStarted(machine.Id, cancellationToken).ConfigureAwait(false);

        // Build a lightweight context (no historical snapshots for on-demand triggers).
        var ctx = new DiagnosticContext(
            machine.MachineName, machine.OsVersion, machine.CpuModel,
            machine.RamGb, machine.DiskTotalGb,
            0, 0, 0, 0, 0, 0, 0, "[]", null, true, true, 0);

        var prompt = DiagnosticPromptBuilder.Build(ctx);
        var responseJson = await claudeApi.AnalyzeAsync(prompt, cancellationToken)
            .ConfigureAwait(false);
        var result = DiagnosticResultParser.Parse(responseJson);

        var report = DiagnosticReport.Create(
            machine.Id, result.Severity,
            JsonSerializer.Serialize(result.Issues),
            JsonSerializer.Serialize(result.Fixes),
            result.Summary);

        diagnosticsDb.DiagnosticReports.Add(report);
        await diagnosticsDb.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        await publisher.NotifyCompleted(machine.Id, result.Severity,
            result.Issues.Count, result.Summary, cancellationToken).ConfigureAwait(false);

        LogTriggered(logger, machine.Id, result.Severity);
        return report.Id;
    }

    [LoggerMessage(Level = LogLevel.Information,
        Message = "Diagnostic triggered for machine {MachineId} severity={Severity}")]
    private static partial void LogTriggered(
        ILogger logger, Guid machineId, Contracts.Dtos.DiagnosticSeverity severity);
}
