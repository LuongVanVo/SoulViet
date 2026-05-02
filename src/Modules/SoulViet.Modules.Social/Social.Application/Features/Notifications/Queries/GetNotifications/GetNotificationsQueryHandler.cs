using MediatR;
using SoulViet.Modules.Social.Social.Application.Features.Notifications.Results;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;
using SoulViet.Modules.Social.Social.Application.Interfaces.Services;
using SoulViet.Modules.Social.Social.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SoulViet.Modules.Social.Social.Application.Features.Notifications.Queries.GetNotifications
{
    public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, List<NotificationResponse>>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserService _userService;

        public GetNotificationsQueryHandler(
            INotificationRepository notificationRepository,
            IUserService userService)
        {
            _notificationRepository = notificationRepository;
            _userService = userService;
        }

        public async Task<List<NotificationResponse>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
        {
            var notifications = await _notificationRepository.GetByUserIdAsync(request.UserId, request.Limit, cancellationToken);
            
            if (notifications == null || !notifications.Any())
            {
                return new List<NotificationResponse>();
            }

            var actorIds = notifications.Select(n => n.ActorUserId).Distinct().ToList();
            var users = await _userService.GetUsersMinimalInfoAsync(actorIds, cancellationToken);

            return notifications.Select(n => new NotificationResponse
            {
                Id = n.Id,
                Type = n.Type,
                TargetType = n.TargetType,
                TargetId = n.TargetId,
                ActorId = n.ActorUserId,
                ActorName = users.ContainsKey(n.ActorUserId) ? users[n.ActorUserId].FullName ?? "User" : "User",
                ActorAvatar = users.ContainsKey(n.ActorUserId) ? users[n.ActorUserId].AvatarUrl ?? string.Empty : string.Empty,
                Message = n.Message ?? string.Empty,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            }).ToList();
        }
    }
}
