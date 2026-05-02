using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoulViet.Modules.Social.Social.Application.Features.Notifications.Queries.GetNotifications;
using SoulViet.Modules.Social.Social.Application.Features.Notifications.Queries.GetUnreadCount;
using SoulViet.Modules.Social.Social.Application.Features.Notifications.Commands.MarkAsRead;
using SoulViet.Modules.Social.Social.Application.Features.Notifications.Commands.MarkAllAsRead;
using SoulViet.Modules.Social.Social.Presentation.Helpers;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SoulViet.Modules.Social.Social.Presentation.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get notifications history", Description = "Returns a list of recent notifications for the current user.")]
        public async Task<IActionResult> GetNotifications([FromQuery] int limit = 20, CancellationToken cancellationToken = default)
        {
            var userId = User.GetCurrentUserId();
            var query = new GetNotificationsQuery(userId, limit);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("unread-count")]
        [SwaggerOperation(Summary = "Get unread count")]
        public async Task<IActionResult> GetUnreadCount(CancellationToken cancellationToken)
        {
            var userId = User.GetCurrentUserId();
            var result = await _mediator.Send(new GetUnreadCountQuery(userId), cancellationToken);
            return Ok(result);
        }

        [HttpPut("{id:guid}/read")]
        [SwaggerOperation(Summary = "Mark a notification as read")]
        public async Task<IActionResult> MarkAsRead([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var userId = User.GetCurrentUserId();
            var result = await _mediator.Send(new MarkNotificationAsReadCommand(id, userId), cancellationToken);
            return result ? Ok() : NotFound();
        }

        [HttpPut("read-all")]
        [SwaggerOperation(Summary = "Mark all notifications as read")]
        public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken)
        {
            var userId = User.GetCurrentUserId();
            await _mediator.Send(new MarkAllNotificationsAsReadCommand(userId), cancellationToken);
            return Ok();
        }
    }
}
