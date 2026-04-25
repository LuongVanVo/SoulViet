using System;
using System.Collections.Generic;
using System.Text;

namespace SoulViet.Modules.Social.Social.Application.Interfaces.Repositories
{
    public interface INotificationService
    {
        Task SendNotificationToUserAsync(Guid userId, object notification);
        Task BroadcastNotificationAsync(object notification);
    }
}
