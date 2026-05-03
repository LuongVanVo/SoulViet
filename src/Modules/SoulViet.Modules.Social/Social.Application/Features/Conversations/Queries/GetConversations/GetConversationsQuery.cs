using MediatR;
using SoulViet.Modules.Social.Social.Domain.Entities;

namespace SoulViet.Modules.Social.Social.Application.Features.Conversations.Queries.GetConversations
{
    public class GetConversationsQuery : IRequest<List<ConversationDto>>
    {
        public Guid UserId { get; set; }
    }

    public class ConversationDto
    {
        public Guid Id { get; set; }
        public Guid TargetUserId { get; set; }
        public string? TargetUserName { get; set; }
        public string? TargetUserAvatar { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public string? LastMessageContent { get; set; }
        public int UnreadCount { get; set; }
    }
}
