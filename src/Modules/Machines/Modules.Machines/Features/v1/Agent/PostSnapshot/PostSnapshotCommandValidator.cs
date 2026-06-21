using FluentValidation;
using MIT.Modules.Machines.Contracts.v1.Machines;

namespace MIT.Modules.Machines.Features.v1.Agent.PostSnapshot;

public sealed class PostSnapshotCommandValidator : AbstractValidator<PostSnapshotCommand>
{
    public PostSnapshotCommandValidator()
    {
        RuleFor(x => x.MachineId).NotEmpty();
        RuleFor(x => x.HardwareJson).NotEmpty();
        RuleFor(x => x.SoftwareListJson).NotEmpty();
    }
}
