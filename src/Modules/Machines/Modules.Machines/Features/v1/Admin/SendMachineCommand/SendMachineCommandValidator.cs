using FluentValidation;
using MIT.Modules.Machines.Contracts.v1.Machines;

namespace MIT.Modules.Machines.Features.v1.Admin.SendMachineCommand;

public sealed class SendMachineCommandValidator : AbstractValidator<SendMachineCommandCommand>
{
    public SendMachineCommandValidator()
    {
        RuleFor(x => x.MachineId).NotEmpty();
        RuleFor(x => x.PayloadJson).NotEmpty();
    }
}
