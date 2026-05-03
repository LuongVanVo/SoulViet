using MediatR;
using Microsoft.EntityFrameworkCore;
using SoulViet.Modules.Social.Social.Domain.Entities;
using SoulViet.Modules.Social.Social.Infrastructure.Persistence;

namespace SoulViet.Modules.Social.Social.Application.Features.Conversations.Commands.GetOrCreateConversation
{
    public class GetOrCreateConversationCommandHandler : IRequestHandler<GetOrCreateConversationCommand, Guid>
    {
        private readonly SocialDbContext _dbContext;

        public GetOrCreateConversationCommandHandler(SocialDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> Handle(GetOrCreateConversationCommand request, CancellationToken cancellationToken)
        {
            if (request.UserId == request.TargetUserId)
            {
                throw new ArgumentException("Cannot create a conversation with yourself.");
            }

            var userAId = request.UserId < request.TargetUserId ? request.UserId : request.TargetUserId;
            var userBId = request.UserId < request.TargetUserId ? request.TargetUserId : request.UserId;

            var conversation = await _dbContext.Conversations
                .FirstOrDefaultAsync(c => c.UserAId == userAId && c.UserBId == userBId, cancellationToken);

            if (conversation != null)
            {
                return conversation.Id;
            }

            conversation = new Conversation
            {
                UserAId = userAId,
                UserBId = userBId
            };

            await _dbContext.Conversations.AddAsync(conversation, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return conversation.Id;
        }
    }
}
