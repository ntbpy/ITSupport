using FluentValidation;
using MIT.Modules.Chat.Contracts.v1.Commands;

namespace MIT.Modules.Chat.Features.v1.Channels.AddChannelMembers;

public sealed class AddChannelMembersCommandValidator : AbstractValidator<AddChannelMembersCommand>
{
    public AddChannelMembersCommandValidator()
    {
        RuleFor(x => x.ChannelId).NotEmpty();
        RuleFor(x => x.UserIds).NotNull().NotEmpty();
        RuleForEach(x => x.UserIds).NotEmpty().MaximumLength(64);
    }
}
