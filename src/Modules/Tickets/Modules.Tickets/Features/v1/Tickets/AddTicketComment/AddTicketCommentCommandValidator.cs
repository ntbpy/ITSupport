using FluentValidation;
using MIT.Modules.Tickets.Contracts.v1.Tickets;

namespace MIT.Modules.Tickets.Features.v1.Tickets.AddTicketComment;

public sealed class AddTicketCommentCommandValidator : AbstractValidator<AddTicketCommentCommand>
{
    public AddTicketCommentCommandValidator()
    {
        RuleFor(x => x.Body).NotEmpty().MaximumLength(8192);
    }
}
