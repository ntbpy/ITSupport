using FluentValidation;
using MIT.Modules.Chat.Contracts.v1.Commands;

namespace MIT.Modules.Chat.Features.v1.Channels.RestoreChannel;

public sealed class RestoreChannelCommandValidator : AbstractValidator<RestoreChannelCommand>
{
    public RestoreChannelCommandValidator()
    {
        RuleFor(x => x.ChannelId).NotEmpty();
    }
}
