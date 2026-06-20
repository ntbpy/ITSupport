using FluentValidation;
using MIT.Modules.Tickets.Contracts.v1.Tickets;

namespace MIT.Modules.Tickets.Features.v1.Tickets.CloseTicket;

public sealed class CloseTicketCommandValidator : AbstractValidator<CloseTicketCommand>
{
    public CloseTicketCommandValidator()
    {
        RuleFor(x => x.TicketId).NotEmpty();
    }
}
