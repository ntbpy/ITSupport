using MIT.Framework.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MIT.Modules.Machines.Data;

public sealed class MachinesDbInitializer(
    MachinesDbContext dbContext,
    ILogger<MachinesDbInitializer> logger) : IDbInitializer
{
    public async Task MigrateAsync(CancellationToken cancellationToken)
    {
        var pending = await dbContext.Database
            .GetPendingMigrationsAsync(cancellationToken)
            .ConfigureAwait(false);

        if (pending.Any())
        {
            logger.LogInformation("[Machines] applying pending migrations");
            await dbContext.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    public Task SeedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
