using FluentValidation;
using MIT.Modules.Machines.Contracts.v1.Machines;

namespace MIT.Modules.Machines.Features.v1.Agent.Heartbeat;

public sealed class HeartbeatCommandValidator : AbstractValidator<HeartbeatCommand>
{
    public HeartbeatCommandValidator()
    {
        RuleFor(x => x.MachineId).NotEmpty();
        RuleFor(x => x.CpuUsagePct).InclusiveBetween(0, 100);
        RuleFor(x => x.RamUsedGb).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DiskUsedGb).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DiskFreePct).InclusiveBetween(0, 100);
    }
}
