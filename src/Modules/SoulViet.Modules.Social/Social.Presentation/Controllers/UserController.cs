using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoulViet.Modules.Social.Social.Application.Features.Users.Queries.GetPublicProfile;
using SoulViet.Modules.Social.Social.Presentation.Helpers;
using Swashbuckle.AspNetCore.Annotations;

namespace SoulViet.Modules.Social.Social.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{userId:guid}/profile")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get user public profile", Description = "Retrieves public profile information for a specific user.")]
        public async Task<IActionResult> GetProfile(Guid userId, CancellationToken cancellationToken)
        {
            Guid? currentUserId = null;
            if (User.Identity?.IsAuthenticated == true)
            {
                currentUserId = User.GetCurrentUserId();
            }

            var query = new GetPublicProfileQuery
            {
                UserId = userId,
                CurrentUserId = currentUserId
            };

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
    }
}
