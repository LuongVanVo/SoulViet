using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SoulViet.Modules.Social.Presentation.Hubs;
using SoulViet.Modules.Social.Social.Infrastructure.Persistence;

namespace SoulViet.Modules.Social.Social.Application.Features.Conversations.Commands.MarkAsRead
{
    public class MarkAsReadCommandHandler : IRequestHandler<MarkAsReadCommand>
    {
        private readonly SocialDbContext _dbContext;
        private readonly IHubContext<NotificationHub> _hubContext;

        public MarkAsReadCommandHandler(SocialDbContext dbContext, IHubContext<NotificationHub> hubContext)
        {
            _dbContext = dbContext;
            _hubContext = hubContext;
        }

        public async Task Handle(MarkAsReadCommand request, CancellationToken cancellationToken)
        {
            var conversation = await _dbContext.Conversations
                .FirstOrDefaultAsync(c => c.Id == request.ConversationId, cancellationToken);

            if (conversation == null) return;

            Guid receiverId;
            if (conversation.UserAId == request.UserId)
            {
                conversation.LastReadMessageIdA = request.LastReadMessageId;
                receiverId = conversation.UserBId;
            }
            else if (conversation.UserBId == request.UserId)
            {
                conversation.LastReadMessageIdB = request.LastReadMessageId;
                receiverId = conversation.UserAId;
            }
            else
            {
                return; // User not in conversation
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            var readReceipt = new
            {
                conversationId = request.ConversationId,
                readByUserId = request.UserId,
                lastReadMessageId = request.LastReadMessageId,
                readAt = DateTime.UtcNow
            };

            await _hubContext.Clients.User(receiverId.ToString())
                .SendAsync("ReadReceipt", readReceipt, cancellationToken: cancellationToken);
        }
    }
}
