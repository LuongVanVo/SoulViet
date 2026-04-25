using MassTransit;
using SoulViet.Shared.Application.Common.Events;
using SoulViet.Modules.Social.Social.Application.Interfaces.Services;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;

namespace SoulViet.Modules.Social.Social.Infrastructure.Consumer
{
    /// <summary>
    /// Consumes PostLikedEvent and PostSharedEvent from RabbitMQ.
    /// Sends real-time SignalR notifications to the post owner via INotificationService.
    /// This decouples the notification logic from the Like/Share handlers.
    /// </summary>
    public class NotificationConsumer :
        IConsumer<PostLikedEvent>,
        IConsumer<PostSharedEvent>,
        IConsumer<UserFollowedEvent>
    {
        private readonly INotificationService _notificationService;

        public NotificationConsumer(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task Consume(ConsumeContext<PostLikedEvent> context)
        {
            var message = context.Message;
            var notification = new
            {
                Type = "Like",
                ActorId = message.ActorId,
                ActorName = message.ActorName,
                Message = $"{message.ActorName} đã thích bài viết của bạn.",
                PostId = message.PostId,
                CreatedAt = message.CreatedAt
            };

            await _notificationService.SendNotificationToUserAsync(message.PostOwnerId, notification);
        }

        public async Task Consume(ConsumeContext<PostSharedEvent> context)
        {
            var message = context.Message;

            string notifMessage = string.IsNullOrWhiteSpace(message.Caption)
                ? $"{message.ActorName} đã chia sẻ bài viết của bạn."
                : $"{message.ActorName} đã chia sẻ bài viết của bạn: \"{message.Caption}\"";

            var notification = new
            {
                Type = "Share",
                ActorId = message.ActorId,
                ActorName = message.ActorName,
                Message = notifMessage,
                PostId = message.PostId,
                ShareId = message.ShareId,
                ShareType = message.ShareType.ToString(),
                CreatedAt = message.CreatedAt
            };

            await _notificationService.SendNotificationToUserAsync(message.PostOwnerId, notification);
        }

        public async Task Consume(ConsumeContext<UserFollowedEvent> context)
        {
            var message = context.Message;
            var notification = new
            {
                Type = "Follow",
                ActorId = message.FollowerId,
                ActorName = message.FollowerName,
                Message = $"{message.FollowerName} đã bắt đầu theo dõi bạn.",
                CreatedAt = message.CreatedAt
            };

            await _notificationService.SendNotificationToUserAsync(message.FollowingId, notification);
        }
    }
}
