using Microsoft.AspNetCore.SignalR;
using SoulViet.Modules.Social.Presentation.Hubs;
using SoulViet.Modules.Social.Social.Application.Interfaces.Services;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;
using SoulViet.Modules.Social.Social.Application.Interfaces;
using SoulViet.Shared.Domain.Enums;
using SoulViet.Modules.Social.Social.Domain.Entities;

namespace SoulViet.Modules.Social.Social.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly INotificationRepository _notificationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(
            IHubContext<NotificationHub> hubContext,
            INotificationRepository notificationRepository,
            IUnitOfWork unitOfWork)
        {
            _hubContext = hubContext;
            _notificationRepository = notificationRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task SendNotificationAsync(Guid recipientId, Guid actorId, NotificationType type, NotificationTargetType targetType, Guid? targetId, string message, CancellationToken cancellationToken = default)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                RecipientUserId = recipientId,
                ActorUserId = actorId,
                Type = type,
                TargetType = targetType,
                TargetId = targetId,
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationRepository.AddAsync(notification, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _hubContext.Clients.Group(recipientId.ToString())
                .SendAsync("ReceiveNotification", new
                {
                    Id = notification.Id,
                    Type = type.ToString(),
                    TargetType = targetType.ToString(),
                    ActorId = actorId,
                    Message = message,
                    TargetId = targetId,
                    CreatedAt = notification.CreatedAt,
                    IsRead = false
                }, cancellationToken);
        }

        public async Task BroadcastNotificationAsync(string message, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients.All
                .SendAsync("ReceiveNotification", new { Message = message, CreatedAt = DateTime.UtcNow }, cancellationToken);
        }
    }
}
