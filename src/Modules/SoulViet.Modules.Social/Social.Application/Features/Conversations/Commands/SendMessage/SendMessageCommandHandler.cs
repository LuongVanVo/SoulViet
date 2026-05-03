using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SoulViet.Modules.Social.Social.Infrastructure.Persistence;
using SoulViet.Shared.Application.Common.Events;

namespace SoulViet.Modules.Social.Social.Application.Features.Conversations.Commands.SendMessage
{
    public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand>
    {
        private readonly SocialDbContext _dbContext;
        private readonly IPublishEndpoint _publishEndpoint;

        public SendMessageCommandHandler(SocialDbContext dbContext, IPublishEndpoint publishEndpoint)
        {
            _dbContext = dbContext;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            var conversation = await _dbContext.Conversations
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.ConversationId, cancellationToken);

            if (conversation == null) throw new Exception("Conversation not found");
            if (conversation.UserAId != request.SenderId && conversation.UserBId != request.SenderId)
                throw new Exception("User not in conversation");

            var receiverId = conversation.UserAId == request.SenderId ? conversation.UserBId : conversation.UserAId;

            var chatEvent = new ChatMessageEvent
            {
                IdempotencyKey = Guid.NewGuid(),
                ConversationId = request.ConversationId,
                SenderId = request.SenderId,
                ReceiverId = receiverId,
                Type = request.Type,
                Content = request.Content,
                MediaUrl = request.MediaUrl,
                ClientTempId = request.ClientTempId
            };

            await _publishEndpoint.Publish(chatEvent, cancellationToken);
        }
    }
}
