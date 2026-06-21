using FluentValidation;
using MIT.Modules.Diagnostics.Contracts.v1.Diagnostics;

namespace MIT.Modules.Diagnostics.Features.v1.TriggerDiagnostic;

public sealed class TriggerDiagnosticCommandValidator : AbstractValidator<TriggerDiagnosticCommand>
{
    public TriggerDiagnosticCommandValidator()
    {
        RuleFor(x => x.MachineId).NotEmpty();
    }
}
