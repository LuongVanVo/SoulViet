using System;
using System.Collections.Generic;
using System.Text;

namespace SoulViet.Modules.Social.Social.Application.Interfaces.Repositories
{
    public interface INotificationService
    {
        Task SendNotificationAsync(Guid recipientId, Guid actorId, SoulViet.Shared.Domain.Enums.NotificationType type, SoulViet.Shared.Domain.Enums.NotificationTargetType targetType, Guid? targetId, string message, CancellationToken cancellationToken = default);
        Task BroadcastNotificationAsync(string message, CancellationToken cancellationToken = default);
    }
}
