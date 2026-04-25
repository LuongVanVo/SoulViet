using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoulViet.Modules.Social.Social.Application.Features.UserFollowers.Commands.FollowUser;
using SoulViet.Modules.Social.Social.Application.Features.UserFollowers.Commands.UnfollowUser;
using SoulViet.Modules.Social.Social.Application.Features.UserFollowers.Queries.GetFollowers;
using SoulViet.Modules.Social.Social.Application.Features.UserFollowers.Queries.GetFollowing;
using SoulViet.Modules.Social.Social.Presentation.Helpers;
using Swashbuckle.AspNetCore.Annotations;

namespace SoulViet.Modules.Social.Social.Presentation.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class FollowerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FollowerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("{followingId:guid}")]
        [SwaggerOperation(Summary = "Follow user", Description = "Follows another user.")]
        public async Task<IActionResult> Follow(Guid followingId, CancellationToken cancellationToken)
        {
            var command = new FollowUserCommand
            {
                FollowerId = User.GetCurrentUserId(),
                FollowingId = followingId
            };

            var result = await _mediator.Send(command, cancellationToken);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{followingId:guid}")]
        [SwaggerOperation(Summary = "Unfollow user", Description = "Unfollows a previously followed user.")]
        public async Task<IActionResult> Unfollow(Guid followingId, CancellationToken cancellationToken)
        {
            var command = new UnfollowUserCommand
            {
                FollowerId = User.GetCurrentUserId(),
                FollowingId = followingId
            };

            var result = await _mediator.Send(command, cancellationToken);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{userId:guid}/followers")]
        [SwaggerOperation(Summary = "Get followers", Description = "Retrieves a keyset paginated list of followers for a specific user.")]
        public async Task<IActionResult> GetFollowers(
            Guid userId,
            [FromQuery] string? after,
            [FromQuery] int first = 20,
            [FromQuery] string sortBy = "newest",
            CancellationToken cancellationToken = default)
        {
            var query = new GetFollowersQuery
            {
                UserId = userId,
                CurrentUserId = User.GetCurrentUserId(),
                After = after,
                First = first,
                SortBy = string.IsNullOrWhiteSpace(sortBy) ? "newest" : sortBy.ToLowerInvariant()
            };

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{userId:guid}/following")]
        [SwaggerOperation(Summary = "Get following", Description = "Retrieves a keyset paginated list of users followed by a specific user.")]
        public async Task<IActionResult> GetFollowing(
            Guid userId,
            [FromQuery] string? after,
            [FromQuery] int first = 20,
            [FromQuery] string sortBy = "newest",
            CancellationToken cancellationToken = default)
        {
            var query = new GetFollowingQuery
            {
                UserId = userId,
                CurrentUserId = User.GetCurrentUserId(),
                After = after,
                First = first,
                SortBy = string.IsNullOrWhiteSpace(sortBy) ? "newest" : sortBy.ToLowerInvariant()
            };

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
    }
}
