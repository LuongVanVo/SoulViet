using SoulViet.Modules.Social.Social.Domain.Common;

namespace SoulViet.Modules.Social.Social.Domain.Entities
{
    public class Conversation : BaseAuditableEntity
    {
        public Guid UserAId { get; set; }
        public Guid UserBId { get; set; }
        public DateTime? LastMessageAt { get; set; }
        
        public Guid? LastReadMessageIdA { get; set; }
        public Guid? LastReadMessageIdB { get; set; }

        public Message? LastReadMessageA { get; set; }
        public Message? LastReadMessageB { get; set; }
        
        public List<Message> Messages { get; set; } = new();
    }
}
