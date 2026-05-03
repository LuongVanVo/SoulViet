using SoulViet.Modules.Social.Social.Domain.Common;
using SoulViet.Modules.Social.Social.Domain.Enums;

namespace SoulViet.Modules.Social.Social.Domain.Entities
{
    public class Message : BaseAuditableEntity
    {
        public Guid ConversationId { get; set; }
        public Conversation Conversation { get; set; } = null!;
        
        public Guid SenderId { get; set; }
        public MessageType Type { get; set; } = MessageType.Text;
        public string? Content { get; set; }
        public string? MediaUrl { get; set; }
        public Guid ClientTempId { get; set; }
        
        public DateTime? DeletedAt { get; set; }
    }
}
