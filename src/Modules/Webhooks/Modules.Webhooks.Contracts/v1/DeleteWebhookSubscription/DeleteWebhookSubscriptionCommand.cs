using Mediator;

namespace MIT.Modules.Webhooks.Contracts.v1.DeleteWebhookSubscription;

public sealed record DeleteWebhookSubscriptionCommand(Guid Id) : ICommand;
