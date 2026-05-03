using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoulViet.Modules.Social.Social.Application.Features.Conversations.Queries.GetConversations;
using SoulViet.Modules.Social.Social.Application.Features.Conversations.Queries.GetMessages;
using SoulViet.Modules.Social.Social.Application.Features.Conversations.Commands.DeleteMessage;
using System.Security.Claims;

namespace SoulViet.Modules.Social.Presentation.Controllers
{
    [ApiController]
    [Route("api/conversations")]
    [Authorize]
    public class ConversationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ConversationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetConversations()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "Id") ?? 
                              User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier) ?? 
                              User.Claims.FirstOrDefault(c => c.Type == "sub");
            var userId = Guid.Parse(userIdClaim?.Value ?? Guid.Empty.ToString());
            if (userId == Guid.Empty) return Unauthorized();

            var query = new GetConversationsQuery { UserId = userId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}/messages")]
        public async Task<IActionResult> GetMessages(Guid id, [FromQuery] Guid? before, [FromQuery] int limit = 30)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "Id") ?? 
                              User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier) ?? 
                              User.Claims.FirstOrDefault(c => c.Type == "sub");
            var userId = Guid.Parse(userIdClaim?.Value ?? Guid.Empty.ToString());
            if (userId == Guid.Empty) return Unauthorized();

            var query = new GetMessagesQuery
            {
                ConversationId = id,
                UserId = userId,
                BeforeMessageId = before,
                Limit = limit
            };
            
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }

    [ApiController]
    [Route("api/messages")]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MessageController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(Guid id)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "Id") ?? 
                              User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier) ?? 
                              User.Claims.FirstOrDefault(c => c.Type == "sub");
            var userId = Guid.Parse(userIdClaim?.Value ?? Guid.Empty.ToString());
            if (userId == Guid.Empty) return Unauthorized();

            var command = new DeleteMessageCommand
            {
                MessageId = id,
                UserId = userId
            };

            await _mediator.Send(command);
            return NoContent();
        }
    }
}
