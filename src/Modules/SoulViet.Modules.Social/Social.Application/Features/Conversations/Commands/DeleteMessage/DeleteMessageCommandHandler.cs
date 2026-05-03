using MediatR;
using Microsoft.EntityFrameworkCore;
using SoulViet.Modules.Social.Social.Infrastructure.Persistence;

namespace SoulViet.Modules.Social.Social.Application.Features.Conversations.Commands.DeleteMessage
{
    public class DeleteMessageCommandHandler : IRequestHandler<DeleteMessageCommand>
    {
        private readonly SocialDbContext _dbContext;

        public DeleteMessageCommandHandler(SocialDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
        {
            var message = await _dbContext.Messages
                .FirstOrDefaultAsync(m => m.Id == request.MessageId && !m.DeletedAt.HasValue, cancellationToken);

            if (message == null) return;
            if (message.SenderId != request.UserId) throw new UnauthorizedAccessException("You can only delete your own messages");

            message.DeletedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
