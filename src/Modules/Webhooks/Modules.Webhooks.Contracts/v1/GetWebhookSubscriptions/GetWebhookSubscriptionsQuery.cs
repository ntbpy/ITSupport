using MIT.Framework.Shared.Persistence;
using MIT.Modules.Webhooks.Contracts.Dtos;
using Mediator;

namespace MIT.Modules.Webhooks.Contracts.v1.GetWebhookSubscriptions;

public sealed record GetWebhookSubscriptionsQuery(int PageNumber = 1, int PageSize = 10)
    : IQuery<PagedResponse<WebhookSubscriptionDto>>;
