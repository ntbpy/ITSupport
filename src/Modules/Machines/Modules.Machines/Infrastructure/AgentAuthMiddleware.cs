using MIT.Modules.Machines.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MIT.Modules.Machines.Infrastructure;

public sealed class AgentAuthMiddleware(RequestDelegate next, ILogger<AgentAuthMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (!context.Request.Path.StartsWithSegments("/api/v1/agent", StringComparison.OrdinalIgnoreCase))
        {
            await next(context).ConfigureAwait(false);
            return;
        }

        if (!context.Request.Headers.TryGetValue("X-Agent-Key", out var apiKey)
            || !context.Request.Headers.TryGetValue("X-Machine-Id", out var machineIdStr)
            || !Guid.TryParse(machineIdStr, out var machineId))
        {
            context.Response.StatusCode = 401;
            return;
        }

        var db = context.RequestServices.GetRequiredService<MachinesDbContext>();
        var keyService = context.RequestServices.GetRequiredService<IAgentApiKeyService>();

        var machine = await db.Machines
            .FirstOrDefaultAsync(m => m.Id == machineId, context.RequestAborted)
            .ConfigureAwait(false);

        if (machine is null || machine.EncryptedApiKey is null
            || !keyService.Verify(apiKey.ToString(), machine.EncryptedApiKey))
        {
            logger.LogWarning("Agent auth failed for machine {MachineId}", machineId);
            context.Response.StatusCode = 401;
            return;
        }

        context.Items["MachineId"] = machineId;
        await next(context).ConfigureAwait(false);
    }
}
