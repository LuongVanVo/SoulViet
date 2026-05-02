using SoulViet.Shared.Domain.Enums;
using System;

namespace SoulViet.Modules.Social.Social.Application.DTOs
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public Guid RecipientUserId { get; set; }
        public Guid ActorUserId { get; set; }
        public NotificationType Type { get; set; }
        public NotificationTargetType TargetType { get; set; }
        public Guid? TargetId { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
