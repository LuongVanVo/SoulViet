using MediatR;
using System;

namespace SoulViet.Modules.Social.Social.Application.Features.Notifications.Queries.GetUnreadCount
{
    public record GetUnreadCountQuery(Guid UserId) : IRequest<int>;
}
