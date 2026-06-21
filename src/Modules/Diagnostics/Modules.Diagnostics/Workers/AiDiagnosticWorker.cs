using MIT.Modules.Diagnostics.Contracts.Dtos;
using MIT.Modules.Diagnostics.Data;
using MIT.Modules.Diagnostics.Domain;
using MIT.Modules.Diagnostics.Infrastructure;
using MIT.Modules.Machines.Contracts.v1.Machines;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace MIT.Modules.Diagnostics.Workers;

public sealed partial class AiDiagnosticWorker(
    IServiceScopeFactory scopeFactory,
    IConnectionMultiplexer redis,
    ILogger<AiDiagnosticWorker> logger) : BackgroundService
{
    private const string StreamKey = "ai_diagnostic_queue";
    private const string ConsumerGroup = "diagnostic_workers";
    private const string ConsumerName = "worker-1";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var db = redis.GetDatabase();
        try
        {
            await db.StreamCreateConsumerGroupAsync(
                StreamKey, ConsumerGroup, StreamPosition.NewMessages).ConfigureAwait(false);
        }
        catch (RedisServerException ex) when (ex.Message.Contains("BUSYGROUP", StringComparison.Ordinal))
        {
            LogGroupAlreadyExists(logger, ex);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var entries = await db.StreamReadGroupAsync(
                    StreamKey, ConsumerGroup, ConsumerName,
                    count: 1, noAck: false).ConfigureAwait(false);

                if (entries.Length == 0)
                {
                    await Task.Delay(2000, stoppingToken).ConfigureAwait(false);
                    continue;
                }

                foreach (var entry in entries)
                {
                    var machineId = Guid.Parse(entry["machineId"].ToString());
                    await ProcessAsync(machineId, stoppingToken).ConfigureAwait(false);
                    await db.StreamAcknowledgeAsync(StreamKey, ConsumerGroup, entry.Id)
                        .ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException) { break; }
            catch (Exception ex)
            {
                LogWorkerError(logger, ex);
                await Task.Delay(30_000, stoppingToken).ConfigureAwait(false);
            }
        }
    }

    private async Task ProcessAsync(Guid machineId, CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var diagnosticsDb = scope.ServiceProvider.GetRequiredService<DiagnosticsDbContext>();
        var claudeApi = scope.ServiceProvider.GetRequiredService<IClaudeApiService>();
        var autoFix = scope.ServiceProvider.GetRequiredService<AutoFixService>();
        var publisher = scope.ServiceProvider.GetRequiredService<IDiagnosticPublisher>();

        var machine = await mediator.Send(new GetMachineDetailQuery(machineId), ct)
            .ConfigureAwait(false);
        if (machine is null) return;

        var snapshots = await mediator.Send(new GetMachineSnapshotsQuery(machineId, 7), ct)
            .ConfigureAwait(false);
        if (snapshots.Count == 0) return;

        var ctx = new DiagnosticContext(
            machine.MachineName, machine.OsVersion, machine.CpuModel,
            machine.RamGb, machine.DiskTotalGb,
            (decimal)snapshots.Average(s => (double)s.CpuUsagePct),
            snapshots.Max(s => s.CpuUsagePct),
            (decimal)snapshots.Average(s => (double)s.RamUsedGb),
            (decimal)(snapshots.Average(s => (double)s.RamUsedGb) / (double)machine.RamGb * 100),
            snapshots[snapshots.Count - 1].DiskUsedGb,
            100 - snapshots[snapshots.Count - 1].DiskFreePct,
            (decimal)snapshots.Average(s => s.EventLogErrors),
            snapshots[snapshots.Count - 1].SoftwareListJson ?? "[]",
            null, true, true, 0);

        await publisher.NotifyStarted(machineId, ct).ConfigureAwait(false);

        var prompt = DiagnosticPromptBuilder.Build(ctx);
        var responseJson = await claudeApi.AnalyzeAsync(prompt, ct).ConfigureAwait(false);
        var result = DiagnosticResultParser.Parse(responseJson);

        var report = DiagnosticReport.Create(machineId, result.Severity,
            JsonSerializer.Serialize(result.Issues),
            JsonSerializer.Serialize(result.Fixes),
            result.Summary);

        diagnosticsDb.DiagnosticReports.Add(report);
        await diagnosticsDb.SaveChangesAsync(ct).ConfigureAwait(false);

        if (result.Severity is DiagnosticSeverity.High or DiagnosticSeverity.Critical)
        {
            await autoFix.ApplyAsync(machineId, result, ct).ConfigureAwait(false);
            report.MarkAutoFixed();
            await diagnosticsDb.SaveChangesAsync(ct).ConfigureAwait(false);
        }

        await publisher.NotifyCompleted(machineId, result.Severity,
            result.Issues.Count, result.Summary, ct).ConfigureAwait(false);

        LogDiagnosticCompleted(logger, machineId, result.Severity);
    }

    [LoggerMessage(Level = LogLevel.Debug, Message = "Consumer group already exists")]
    private static partial void LogGroupAlreadyExists(ILogger logger, Exception ex);

    [LoggerMessage(Level = LogLevel.Error, Message = "AiDiagnosticWorker error; backing off 30s")]
    private static partial void LogWorkerError(ILogger logger, Exception ex);

    [LoggerMessage(Level = LogLevel.Information,
        Message = "Diagnostic completed for machine {MachineId} severity={Severity}")]
    private static partial void LogDiagnosticCompleted(
        ILogger logger, Guid machineId, DiagnosticSeverity severity);
}
