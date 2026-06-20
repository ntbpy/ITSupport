using FluentValidation;
using MIT.Modules.Identity.Contracts.v1.Users.AssignUserRoles;

namespace MIT.Modules.Identity.Features.v1.Users.AssignUserRoles;

public sealed class AssignUserRolesCommandValidator : AbstractValidator<AssignUserRolesCommand>
{
    public AssignUserRolesCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.UserRoles)
            .NotNull().WithMessage("User roles list is required.");
    }
}