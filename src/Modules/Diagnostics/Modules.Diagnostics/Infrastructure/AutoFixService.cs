using MIT.Modules.Machines.Contracts.Dtos;
using MIT.Modules.Machines.Contracts.v1.Machines;
using Mediator;
using System.Text.Json;

namespace MIT.Modules.Diagnostics.Infrastructure;

internal static class SafeFixCommands
{
    public static readonly HashSet<string> Patterns =
    [
        "disk cleanup", "clear temp", "restart av service", "clear prefetch"
    ];

    public static bool IsSafe(string fixCommand) =>
        Patterns.Any(p => fixCommand.Contains(p, StringComparison.OrdinalIgnoreCase));
}

public sealed class AutoFixService(IMediator mediator)
{
    public async Task ApplyAsync(
        Guid machineId, DiagnosticResult result, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(result);

        foreach (var fix in result.Fixes.Where(f => f.AutoFixable && f.FixCommand is not null))
        {
            var payload = JsonSerializer.Serialize(new
            {
                script = fix.FixCommand,
                title = fix.Title,
                requiresApproval = !SafeFixCommands.IsSafe(fix.FixCommand!)
            });
            await mediator.Send(
                new SendMachineCommandCommand(machineId, CommandType.Fix, payload), ct)
                .ConfigureAwait(false);
        }
    }
}
