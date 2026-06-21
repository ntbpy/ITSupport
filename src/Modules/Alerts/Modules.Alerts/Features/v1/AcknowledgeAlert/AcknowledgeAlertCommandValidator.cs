using FluentValidation;
using MIT.Modules.Alerts.Contracts.v1.Alerts;

namespace MIT.Modules.Alerts.Features.v1.AcknowledgeAlert;

public sealed class AcknowledgeAlertCommandValidator : AbstractValidator<AcknowledgeAlertCommand>
{
    public AcknowledgeAlertCommandValidator()
    {
        RuleFor(x => x.AlertId).NotEmpty();
    }
}
