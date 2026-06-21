using FluentValidation;
using MIT.Modules.Machines.Contracts.v1.Machines;

namespace MIT.Modules.Machines.Features.v1.Agent.RegisterAgent;

public sealed class RegisterAgentCommandValidator : AbstractValidator<RegisterAgentCommand>
{
    public RegisterAgentCommandValidator()
    {
        RuleFor(x => x.MachineName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.MacAddress).NotEmpty()
            .Matches(@"^([0-9A-Fa-f]{2}[:\-]){5}[0-9A-Fa-f]{2}$")
            .WithMessage("MacAddress must be a valid MAC address");
        RuleFor(x => x.IpAddress).NotEmpty().MaximumLength(45);
        RuleFor(x => x.RamGb).GreaterThan(0);
        RuleFor(x => x.DiskTotalGb).GreaterThan(0);
    }
}
