using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;
using SoulViet.Shared.Application.Common.Events;
using SoulViet.Modules.Social.Social.Infrastructure.Persistence;
using SoulViet.Modules.Social.Social.Domain.Entities;
using SoulViet.Modules.Social.Presentation.Hubs;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;
using SoulViet.Shared.Domain.Enums;
using SoulViet.Modules.Social.Social.Domain.Enums;  
using System.Linq;

namespace SoulViet.Modules.Social.Social.Infrastructure.Consumer
{
    public class ChatMessageConsumer : IConsumer<ChatMessageEvent>
    {
        private readonly SocialDbContext _dbContext;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IConnectionMultiplexer _redis;
        private readonly INotificationService _notificationService;

        public ChatMessageConsumer(
            SocialDbContext dbContext,
            IHubContext<NotificationHub> hubContext,
            IConnectionMultiplexer redis,
            INotificationService notificationService)
        {
            _dbContext = dbContext;
            _hubContext = hubContext;
            _redis = redis;
            _notificationService = notificationService;
        }

        public async Task Consume(ConsumeContext<ChatMessageEvent> context)
        {
            var message = context.Message;
            var db = _redis.GetDatabase();
            var redisKey = $"chat:processed:{message.IdempotencyKey}";

            if (await db.KeyExistsAsync(redisKey))
            {
                return; // ACK và bỏ qua
            }

            var conversation = await _dbContext.Conversations
                .FirstOrDefaultAsync(c => c.Id == message.ConversationId, context.CancellationToken);

            if (conversation == null) return; 
            if (conversation.UserAId != message.SenderId && conversation.UserBId != message.SenderId)
                return; 

            var newMessage = new Message
            {
                Id = Guid.NewGuid(),
                ConversationId = message.ConversationId,
                SenderId = message.SenderId,
                Type = (MessageType)message.Type,
                Content = message.Content,
                MediaUrl = message.MediaUrl,
                ClientTempId = message.ClientTempId,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Messages.Add(newMessage);
            conversation.LastMessageAt = DateTime.UtcNow;

            try
            {
                await _dbContext.SaveChangesAsync(context.CancellationToken);
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505")
            {
                // ACK và bỏ qua
                return; 
            }

            await db.StringSetAsync(redisKey, "1", TimeSpan.FromDays(1));

            var messageDto = new
            {
                id = newMessage.Id,
                clientTempId = message.ClientTempId,
                conversationId = message.ConversationId,
                senderId = message.SenderId,
                type = newMessage.Type,
                content = message.Content,
                mediaUrl = message.MediaUrl,
                createdAt = newMessage.CreatedAt
            };

            await _hubContext.Clients.Users(message.SenderId.ToString(), message.ReceiverId.ToString())
                .SendAsync("MessageSent", messageDto);

            var presenceKey = $"presence:{message.ReceiverId}";
            if (!await db.KeyExistsAsync(presenceKey))
            {
                var contentStr = message.Content ?? (message.Type == 1 ? "[Image]" : (message.Type == 2 ? "[Video]" : ""));
                var contentPreview = contentStr.Length > 50 ? contentStr.Substring(0, 50) + "..." : contentStr;
                await _notificationService.SendNotificationAsync(
                    recipientId: message.ReceiverId,
                    actorId: message.SenderId,
                    type: NotificationType.Message,
                    targetType: NotificationTargetType.Message,
                    targetId: message.ConversationId,
                    message: contentPreview,
                    cancellationToken: context.CancellationToken
                );
            }
        }
    }
}
