using MIT.Framework.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MIT.Modules.Diagnostics.Data;

internal sealed partial class DiagnosticsDbInitializer(
    DiagnosticsDbContext dbContext,
    ILogger<DiagnosticsDbInitializer> logger) : IDbInitializer
{
    public async Task MigrateAsync(CancellationToken cancellationToken)
    {
        var pending = await dbContext.Database.GetPendingMigrationsAsync(cancellationToken)
            .ConfigureAwait(false);
        if (pending.Any())
        {
            LogApplyingMigrations(logger);
            await dbContext.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    public Task SeedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    [LoggerMessage(Level = LogLevel.Information, Message = "[Diagnostics] applying pending migrations")]
    private static partial void LogApplyingMigrations(ILogger logger);
}
