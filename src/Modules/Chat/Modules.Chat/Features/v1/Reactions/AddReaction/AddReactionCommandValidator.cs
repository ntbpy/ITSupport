using FluentValidation;
using MIT.Modules.Chat.Contracts.v1.Commands;

namespace MIT.Modules.Chat.Features.v1.Reactions.AddReaction;

public sealed class AddReactionCommandValidator : AbstractValidator<AddReactionCommand>
{
    public AddReactionCommandValidator()
    {
        RuleFor(x => x.MessageId).NotEmpty();
        RuleFor(x => x.Emoji).NotEmpty().MaximumLength(64);
    }
}
