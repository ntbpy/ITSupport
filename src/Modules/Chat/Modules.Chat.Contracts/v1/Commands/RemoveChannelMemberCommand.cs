using Mediator;

namespace MIT.Modules.Chat.Contracts.v1.Commands;

public sealed record RemoveChannelMemberCommand(
    Guid ChannelId,
    string UserId) : ICommand<Unit>;
