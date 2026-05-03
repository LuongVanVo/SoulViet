using MediatR;

namespace SoulViet.Modules.Social.Social.Application.Features.Conversations.Commands.SendMessage
{
    public class SendMessageCommand : IRequest
    {
        public Guid ConversationId { get; set; }
        public Guid SenderId { get; set; }
        public string? Content { get; set; }
        public int Type { get; set; } // 0 = Text, 1 = Image, 2 = Video
        public string? MediaUrl { get; set; }
        public Guid ClientTempId { get; set; }
    }
}
