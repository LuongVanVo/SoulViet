using MediatR;
using Microsoft.EntityFrameworkCore;
using SoulViet.Modules.Social.Social.Infrastructure.Persistence;

namespace SoulViet.Modules.Social.Social.Application.Features.Conversations.Queries.GetMessages
{
    public class GetMessagesQueryHandler : IRequestHandler<GetMessagesQuery, GetMessagesResult>
    {
        private readonly SocialDbContext _dbContext;

        public GetMessagesQueryHandler(SocialDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<GetMessagesResult> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
        {
            var conversation = await _dbContext.Conversations
                .FirstOrDefaultAsync(c => c.Id == request.ConversationId, cancellationToken);

            if (conversation == null || (conversation.UserAId != request.UserId && conversation.UserBId != request.UserId))
            {
                throw new UnauthorizedAccessException("You are not part of this conversation");
            }

            var query = _dbContext.Messages
                .Where(m => m.ConversationId == request.ConversationId && !m.DeletedAt.HasValue);

            if (request.BeforeMessageId.HasValue)
            {
                var beforeMessage = await _dbContext.Messages
                    .FirstOrDefaultAsync(m => m.Id == request.BeforeMessageId.Value, cancellationToken);

                if (beforeMessage != null)
                {
                    query = query.Where(m => m.CreatedAt < beforeMessage.CreatedAt);
                }
            }

            var messages = await query
                .OrderByDescending(m => m.CreatedAt)
                .Take(request.Limit)
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    ConversationId = m.ConversationId,
                    SenderId = m.SenderId,
                    Type = (int)m.Type,
                    Content = m.Content,
                    MediaUrl = m.MediaUrl,
                    ClientTempId = m.ClientTempId,
                    CreatedAt = m.CreatedAt
                })
                .ToListAsync(cancellationToken);

            var lastReadMessageId = conversation.UserAId == request.UserId ? conversation.LastReadMessageIdA : conversation.LastReadMessageIdB;

            return new GetMessagesResult
            {
                Messages = messages,
                LastReadMessageId = lastReadMessageId
            };
        }
    }
}
