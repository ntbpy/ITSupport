using Mediator;

namespace MIT.Modules.Notifications.Contracts.v1.Commands;

public sealed record MarkNotificationReadCommand(Guid NotificationId) : ICommand<Unit>;
