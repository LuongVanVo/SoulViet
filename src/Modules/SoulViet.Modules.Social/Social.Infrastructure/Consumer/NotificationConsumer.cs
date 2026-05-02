using SoulViet.Shared.Domain.Enums;
using MassTransit;
using SoulViet.Shared.Application.Common.Events;
using SoulViet.Modules.Social.Social.Application.Interfaces.Services;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;

namespace SoulViet.Modules.Social.Social.Infrastructure.Consumer
{
    public class NotificationConsumer :
        IConsumer<PostLikedEvent>,
        IConsumer<PostSharedEvent>,
        IConsumer<UserFollowedEvent>,
        IConsumer<PostCommentedEvent>
    {
        private readonly INotificationService _notificationService;

        public NotificationConsumer(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task Consume(ConsumeContext<PostLikedEvent> context)
        {
            var message = context.Message;
            string notifMessage = $"{message.ActorName} đã thích bài viết của bạn.";

            await _notificationService.SendNotificationAsync(
                message.PostOwnerId,
                message.ActorId,
                NotificationType.Liked,
                NotificationTargetType.Post,
                message.PostId,
                notifMessage,
                context.CancellationToken);
        }

        public async Task Consume(ConsumeContext<PostSharedEvent> context)
        {
            var message = context.Message;

            string notifMessage = $"{message.ActorName} đã chia sẻ bài viết của bạn.";
              
            await _notificationService.SendNotificationAsync(
                message.PostOwnerId,
                message.ActorId,
                NotificationType.Shared,
                NotificationTargetType.Post,
                message.ShareId,
                notifMessage,
                context.CancellationToken);
        }

        public async Task Consume(ConsumeContext<UserFollowedEvent> context)
        {
            var message = context.Message;
            string notifMessage = $"{message.FollowerName} đã bắt đầu theo dõi bạn.";

            await _notificationService.SendNotificationAsync(
                message.FollowingId,
                message.FollowerId,
                NotificationType.Followed,
                NotificationTargetType.User,
                message.FollowerId,
                notifMessage,
                context.CancellationToken);
        }

        public async Task Consume(ConsumeContext<PostCommentedEvent> context)
        {
            var message = context.Message;
            string notifMessage = $"{message.ActorName} đã bình luận về bài viết của bạn.";

            await _notificationService.SendNotificationAsync(
                message.PostOwnerId,
                message.ActorId,
                NotificationType.Commented,
                NotificationTargetType.Post,
                message.PostId,
                notifMessage,
                context.CancellationToken);
        }
    }
}
