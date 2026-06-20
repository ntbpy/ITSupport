using FluentValidation;
using MIT.Modules.Identity.Contracts.v1.Users.DeleteUser;

namespace MIT.Modules.Identity.Features.v1.Users.DeleteUser;

public sealed class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("User ID is required.");
    }
}