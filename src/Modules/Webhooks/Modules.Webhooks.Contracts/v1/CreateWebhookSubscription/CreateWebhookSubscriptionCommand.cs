using Mediator;

namespace MIT.Modules.Webhooks.Contracts.v1.CreateWebhookSubscription;

public sealed record CreateWebhookSubscriptionCommand(
    string Url,
    string[] Events,
    string? Secret) : ICommand<Guid>;
