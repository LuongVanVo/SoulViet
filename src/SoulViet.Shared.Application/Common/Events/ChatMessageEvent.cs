using System;

namespace SoulViet.Shared.Application.Common.Events
{
    public class ChatMessageEvent
    {
        public Guid IdempotencyKey { get; set; }
        public Guid ConversationId { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public int Type { get; set; }
        public string? Content { get; set; }
        public string? MediaUrl { get; set; }
        public Guid ClientTempId { get; set; }
    }
}
