using MediatR;
using Microsoft.EntityFrameworkCore;
using SoulViet.Modules.Social.Social.Infrastructure.Persistence;
using SoulViet.Shared.Application.Interfaces.Repositories;

namespace SoulViet.Modules.Social.Social.Application.Features.Conversations.Queries.GetConversations
{
    public class GetConversationsQueryHandler : IRequestHandler<GetConversationsQuery, List<ConversationDto>>
    {
        private readonly SocialDbContext _dbContext;
        private readonly IUserRepository _userRepository;

        public GetConversationsQueryHandler(SocialDbContext dbContext, IUserRepository userRepository)
        {
            _dbContext = dbContext;
            _userRepository = userRepository;
        }

        public async Task<List<ConversationDto>> Handle(GetConversationsQuery request, CancellationToken cancellationToken)
        {
            var conversations = await _dbContext.Conversations
                .Where(c => c.UserAId == request.UserId || c.UserBId == request.UserId)
                .OrderByDescending(c => c.LastMessageAt ?? c.CreatedAt)
                .ToListAsync(cancellationToken);

            var result = new List<ConversationDto>();

            foreach (var conv in conversations)
            {
                var targetUserId = conv.UserAId == request.UserId ? conv.UserBId : conv.UserAId;
                var targetUser = await _userRepository.GetUserByIdAsync(targetUserId);
                
                var lastReadMessageId = conv.UserAId == request.UserId ? conv.LastReadMessageIdA : conv.LastReadMessageIdB;

                var unreadCountQuery = _dbContext.Messages
                    .Where(m => m.ConversationId == conv.Id && m.SenderId == targetUserId && !m.DeletedAt.HasValue);

                if (lastReadMessageId.HasValue)
                {
                    var lastReadMessage = await _dbContext.Messages.FirstOrDefaultAsync(m => m.Id == lastReadMessageId.Value, cancellationToken);
                    if (lastReadMessage != null)
                    {
                        unreadCountQuery = unreadCountQuery.Where(m => m.CreatedAt > lastReadMessage.CreatedAt);
                    }
                }

                var unreadCount = await unreadCountQuery.CountAsync(cancellationToken);

                var lastMessage = await _dbContext.Messages
                    .Where(m => m.ConversationId == conv.Id && !m.DeletedAt.HasValue)
                    .OrderByDescending(m => m.CreatedAt)
                    .FirstOrDefaultAsync(cancellationToken);

                result.Add(new ConversationDto
                {
                    Id = conv.Id,
                    TargetUserId = targetUserId,
                    TargetUserName = targetUser?.FullName ?? "Unknown User",
                    TargetUserAvatar = targetUser?.AvatarUrl,
                    LastMessageAt = conv.LastMessageAt,
                    LastMessageContent = lastMessage?.Content,
                    UnreadCount = unreadCount
                });
            }

            return result;
        }
    }
}
