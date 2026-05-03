using MediatR;

namespace SoulViet.Modules.Social.Social.Application.Features.Conversations.Commands.MarkAsRead
{
    public class MarkAsReadCommand : IRequest
    {
        public Guid ConversationId { get; set; }
        public Guid UserId { get; set; }
        public Guid LastReadMessageId { get; set; }
    }
}
