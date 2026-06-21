using MIT.Modules.Machines.Contracts.v1.Machines;
using MIT.Modules.Machines.Data;
using MIT.Modules.Machines.Domain;
using MIT.Modules.Machines.Infrastructure;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MIT.Modules.Machines.Features.v1.Agent.RegisterAgent;

public sealed partial class RegisterAgentCommandHandler(
    MachinesDbContext dbContext,
    IAgentApiKeyService keyService,
    ILogger<RegisterAgentCommandHandler> logger)
    : ICommandHandler<RegisterAgentCommand, RegisterAgentResponse>
{
    public async ValueTask<RegisterAgentResponse> Handle(
        RegisterAgentCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var normalizedMac = command.MacAddress.ToUpperInvariant().Trim();
        var existing = await dbContext.Machines
            .FirstOrDefaultAsync(m => m.MacAddress == normalizedMac, cancellationToken)
            .ConfigureAwait(false);

        var plainKey = keyService.Generate();
        var encryptedKey = keyService.Encrypt(plainKey);

        if (existing is not null)
        {
            existing.UpdateInfo(command.MachineName, command.IpAddress, command.OsVersion,
                command.CpuModel, command.RamGb, command.DiskTotalGb, command.AgentVersion);
            existing.SetApiKey(encryptedKey);
            LogAgentReRegistered(logger, existing.Id);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return new RegisterAgentResponse(existing.Id, plainKey, DateTime.UtcNow);
        }

        var machine = Machine.Create(
            command.MachineName, command.IpAddress, normalizedMac,
            command.OsVersion, command.CpuModel, command.RamGb, command.DiskTotalGb,
            command.AgentVersion);
        machine.SetApiKey(encryptedKey);
        dbContext.Machines.Add(machine);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        LogAgentRegistered(logger, machine.Id);
        return new RegisterAgentResponse(machine.Id, plainKey, DateTime.UtcNow);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Agent re-registered for machine {MachineId}")]
    private static partial void LogAgentReRegistered(ILogger logger, Guid machineId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Agent registered new machine {MachineId}")]
    private static partial void LogAgentRegistered(ILogger logger, Guid machineId);
}
