using SoulViet.Shared.Domain.Enums;
using System;

namespace SoulViet.Modules.Social.Social.Application.Features.Notifications.Results
{
    public class NotificationResponse
    {
        public Guid Id { get; set; }
        public NotificationType Type { get; set; }
        public NotificationTargetType TargetType { get; set; }
        public Guid? TargetId { get; set; }
        public Guid ActorId { get; set; }
        public string ActorName { get; set; } = string.Empty;
        public string ActorAvatar { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
