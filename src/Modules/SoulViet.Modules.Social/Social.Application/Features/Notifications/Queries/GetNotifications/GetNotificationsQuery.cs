using MediatR;
using SoulViet.Modules.Social.Social.Application.Features.Notifications.Results;
using System;
using System.Collections.Generic;

namespace SoulViet.Modules.Social.Social.Application.Features.Notifications.Queries.GetNotifications
{
    public record GetNotificationsQuery(Guid UserId, int Limit = 20) : IRequest<List<NotificationResponse>>;
}
