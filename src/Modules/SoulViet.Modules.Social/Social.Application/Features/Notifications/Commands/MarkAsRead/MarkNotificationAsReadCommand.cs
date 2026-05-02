using MediatR;
using System;

namespace SoulViet.Modules.Social.Social.Application.Features.Notifications.Commands.MarkAsRead
{
    public record MarkNotificationAsReadCommand(Guid NotificationId, Guid UserId) : IRequest<bool>;
}
