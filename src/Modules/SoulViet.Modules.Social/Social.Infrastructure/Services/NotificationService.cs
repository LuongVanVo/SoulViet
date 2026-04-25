using Microsoft.AspNetCore.SignalR;
using SoulViet.Modules.Social.Presentation.Hubs;
using SoulViet.Modules.Social.Social.Application.Interfaces.Services;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;

namespace SoulViet.Modules.Social.Social.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendNotificationToUserAsync(Guid userId, object notification)
        {
            await _hubContext.Clients.Group(userId.ToString())
                .SendAsync("ReceiveNotification", notification);
        }
        public async Task BroadcastNotificationAsync(object notification)
        {
            await _hubContext.Clients.All
                .SendAsync("ReceiveNotification", notification);
        }
    }
}
