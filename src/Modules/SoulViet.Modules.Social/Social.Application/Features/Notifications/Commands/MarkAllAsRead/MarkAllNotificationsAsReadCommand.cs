using MediatR;
using System;

namespace SoulViet.Modules.Social.Social.Application.Features.Notifications.Commands.MarkAllAsRead
{
    public record MarkAllNotificationsAsReadCommand(Guid UserId) : IRequest<bool>;
}
