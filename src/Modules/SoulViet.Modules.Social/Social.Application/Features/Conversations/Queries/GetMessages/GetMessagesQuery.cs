using MediatR;
using SoulViet.Modules.Social.Social.Domain.Entities;

namespace SoulViet.Modules.Social.Social.Application.Features.Conversations.Queries.GetMessages
{
    public class GetMessagesQuery : IRequest<GetMessagesResult>
    {
        public Guid ConversationId { get; set; }
        public Guid UserId { get; set; }
        public Guid? BeforeMessageId { get; set; }
        public int Limit { get; set; } = 30;
    }

    public class GetMessagesResult
    {
        public List<MessageDto> Messages { get; set; } = new();
        public Guid? LastReadMessageId { get; set; }
    }

    public class MessageDto
    {
        public Guid Id { get; set; }
        public Guid ConversationId { get; set; }
        public Guid SenderId { get; set; }
        public int Type { get; set; }
        public string? Content { get; set; }
        public string? MediaUrl { get; set; }
        public Guid ClientTempId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
