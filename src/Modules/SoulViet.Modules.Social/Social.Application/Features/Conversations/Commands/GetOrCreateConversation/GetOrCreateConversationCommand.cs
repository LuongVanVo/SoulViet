using MediatR;

namespace SoulViet.Modules.Social.Social.Application.Features.Conversations.Commands.GetOrCreateConversation
{
    public class GetOrCreateConversationCommand : IRequest<Guid>
    {
        public Guid UserId { get; set; }
        public Guid TargetUserId { get; set; }
    }
}
