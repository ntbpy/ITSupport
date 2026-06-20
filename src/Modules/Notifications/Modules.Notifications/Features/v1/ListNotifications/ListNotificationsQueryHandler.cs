using System.Collections.ObjectModel;
using MIT.Framework.Core.Context;
using MIT.Framework.Core.Exceptions;
using MIT.Modules.Notifications.Contracts.v1.DTOs;
using MIT.Modules.Notifications.Contracts.v1.Queries;
using MIT.Modules.Notifications.Data;
using MIT.Modules.Notifications.Features.v1.Internal;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace MIT.Modules.Notifications.Features.v1.ListNotifications;

public sealed class ListNotificationsQueryHandler(
    NotificationsDbContext db,
    ICurrentUser currentUser)
    : IQueryHandler<ListNotificationsQuery, ReadOnlyCollection<NotificationDto>>
{
    public async ValueTask<ReadOnlyCollection<NotificationDto>> Handle(ListNotificationsQuery q, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(q);
        var userId = currentUser.GetUserId();
        if (userId == Guid.Empty) throw new UnauthorizedException("no current user");
        var currentUserId = userId.ToString();

        int page = Math.Max(1, q.Page);
        int pageSize = Math.Clamp(q.PageSize, 1, 200);

        var query = db.Notifications.AsNoTracking()
            .Where(n => n.UserId == currentUserId);

        if (q.UnreadOnly)
        {
            query = query.Where(n => n.ReadAtUtc == null);
        }

        var rows = await query
            .OrderByDescending(n => n.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return rows.Select(n => n.ToDto()).ToList().AsReadOnly();
    }
}
