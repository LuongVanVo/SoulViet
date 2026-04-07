using SoulViet.Modules.Social.Social.Domain.Common;
using SoulViet.Shared.Domain.Enums;
using System;

namespace SoulViet.Modules.Social.Social.Domain.Entities
{
    public class Notification : BaseAuditableEntity
    {
        public Guid RecipientUserId { get; set; }
        public Guid ActorUserId { get; set; }
        public NotificationType Type { get; set; }
        public NotificationTargetType TargetType { get; set; }
        public Guid? TargetId { get; set; }

        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }
    }
}
