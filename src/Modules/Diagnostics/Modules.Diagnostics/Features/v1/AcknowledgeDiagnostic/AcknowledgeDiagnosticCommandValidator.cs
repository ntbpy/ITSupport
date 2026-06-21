using FluentValidation;
using MIT.Modules.Diagnostics.Contracts.v1.Diagnostics;

namespace MIT.Modules.Diagnostics.Features.v1.AcknowledgeDiagnostic;

public sealed class AcknowledgeDiagnosticCommandValidator
    : AbstractValidator<AcknowledgeDiagnosticCommand>
{
    public AcknowledgeDiagnosticCommandValidator()
    {
        RuleFor(x => x.ReportId).NotEmpty();
    }
}
