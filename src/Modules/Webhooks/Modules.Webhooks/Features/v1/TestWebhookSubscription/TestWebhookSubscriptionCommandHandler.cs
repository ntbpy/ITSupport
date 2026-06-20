using MIT.Framework.Core.Exceptions;
using MIT.Modules.Webhooks.Contracts.v1.TestWebhookSubscription;
using MIT.Modules.Webhooks.Data;
using MIT.Modules.Webhooks.Services;
using Mediator;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace MIT.Modules.Webhooks.Features.v1.TestWebhookSubscription;

public sealed class TestWebhookSubscriptionCommandHandler(
    WebhookDbContext dbContext,
    IWebhookDeliveryService deliveryService,
    IWebhookSecretProtector secretProtector) : ICommandHandler<TestWebhookSubscriptionCommand, bool>
{
    public async ValueTask<bool> Handle(TestWebhookSubscriptionCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var subscription = await dbContext.Subscriptions
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == command.Id, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException($"Webhook subscription {command.Id} not found.");

        var testPayload = JsonSerializer.Serialize(new
        {
            eventType = "webhook.test",
            timestamp = TimeProvider.System.GetUtcNow().UtcDateTime,
            message = "This is a test webhook delivery."
        });

        await deliveryService.DeliverAsync(
            subscription.Id,
            subscription.Url,
            secretProtector.Unprotect(subscription.SecretHash),
            "webhook.test",
            testPayload,
            cancellationToken).ConfigureAwait(false);

        return true;
    }
}
