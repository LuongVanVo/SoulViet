using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System;
using MediatR;
using SoulViet.Modules.Social.Social.Application.Features.Conversations.Commands.GetOrCreateConversation;
using SoulViet.Modules.Social.Social.Application.Features.Conversations.Commands.SendMessage;
using SoulViet.Modules.Social.Social.Application.Features.Conversations.Commands.MarkAsRead;

using StackExchange.Redis;

namespace SoulViet.Modules.Social.Presentation.Hubs
{
    [Authorize] 
    public class NotificationHub : Hub
    {
        private readonly IMediator _mediator;
        private readonly IConnectionMultiplexer _redis;

        public NotificationHub(IMediator mediator, IConnectionMultiplexer redis)
        {
            _mediator = mediator;
            _redis = redis;
        }

        public async Task<Guid> GetOrCreateConversation(Guid targetUserId)
        {
            var userId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userId))
            {
                throw new HubException("Unauthorized");
            }

            var command = new GetOrCreateConversationCommand
            {
                UserId = Guid.Parse(userId),
                TargetUserId = targetUserId
            };

            return await _mediator.Send(command);
        }

        public async Task SendMessage(Guid conversationId, string? content, Guid clientTempId, int type = 0, string? mediaUrl = null)
        {
            var userId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userId)) throw new HubException("Unauthorized");

            var command = new SendMessageCommand
            {
                ConversationId = conversationId,
                SenderId = Guid.Parse(userId),
                Content = content,
                Type = type,
                MediaUrl = mediaUrl,
                ClientTempId = clientTempId
            };

            await _mediator.Send(command);
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            if (userId is null)
            {
                Context.Abort();
                return;
            }
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);

            var redisDb = _redis.GetDatabase();
            await redisDb.StringSetAsync($"presence:{userId}", "1", TimeSpan.FromSeconds(45));

            Console.WriteLine($"[SignalR] User '{userId}' connected | ConnectionId: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public async Task Heartbeat()
        {
            var userId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userId)) return;

            var redisDb = _redis.GetDatabase();
            await redisDb.KeyExpireAsync($"presence:{userId}", TimeSpan.FromSeconds(45));
        }

        public async Task MarkAsRead(Guid conversationId, Guid lastReadMessageId)
        {
            var userId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userId)) throw new HubException("Unauthorized");

            var command = new MarkAsReadCommand
            {
                ConversationId = conversationId,
                UserId = Guid.Parse(userId),
                LastReadMessageId = lastReadMessageId
            };
            await _mediator.Send(command);
        }

        public async Task StartTyping(Guid conversationId, Guid targetUserId)
        {
            var userId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userId)) return;

            await Clients.User(targetUserId.ToString()).SendAsync("TypingIndicator", new 
            {
                conversationId = conversationId,
                userId = Guid.Parse(userId),
                isTyping = true
            });
        }

        public async Task StopTyping(Guid conversationId, Guid targetUserId)
        {
            var userId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userId)) return;

            await Clients.User(targetUserId.ToString()).SendAsync("TypingIndicator", new 
            {
                conversationId = conversationId,
                userId = Guid.Parse(userId),
                isTyping = false
            });
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;

            if (userId is not null)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
                
                var redisDb = _redis.GetDatabase();
                await redisDb.KeyDeleteAsync($"presence:{userId}");

                Console.WriteLine($"[SignalR] User '{userId}' disconnected | ConnectionId: {Context.ConnectionId}");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
