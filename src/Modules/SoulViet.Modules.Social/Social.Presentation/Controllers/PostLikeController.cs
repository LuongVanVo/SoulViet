using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoulViet.Modules.Social.Social.Application.Features.PostLikes.Commands.Like;
using SoulViet.Modules.Social.Social.Application.Features.PostLikes.Commands.Unlike;
using SoulViet.Modules.Social.Social.Application.Features.PostLikes.Queries.GetPostLikers;
using SoulViet.Modules.Social.Social.Presentation.Helpers;
using Swashbuckle.AspNetCore.Annotations;

namespace SoulViet.Modules.Social.Social.Presentation.Controllers;

[ApiController]
[Route("api/Post")]
[Authorize]
public class PostLikeController : ControllerBase
{
    private readonly IMediator _mediator;

    public PostLikeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("{postId:guid}/likes")]
    [SwaggerOperation(
        Summary = "Like a post",
        Description = "Adds a like to a specific post and notifies the owner."
    )]
    public async Task<IActionResult> Like(
        [FromRoute] Guid postId,
        CancellationToken cancellationToken)
    {
        var userName = User.FindFirst("full_name")?.Value ?? User.Identity?.Name ?? "User"; 
        var command = new LikePostCommand(postId, User.GetCurrentUserId(), userName);
        
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{postId:guid}/likes")]
    [SwaggerOperation(
        Summary = "Unlike a post",
        Description = "Removes a like from a specific post."
    )]
    public async Task<IActionResult> Unlike(
        [FromRoute] Guid postId,
        CancellationToken cancellationToken)
    {
        var command = new UnlikePostCommand(postId, User.GetCurrentUserId());
        
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{postId:guid}/likers")]
    [AllowAnonymous]
    [SwaggerOperation(
        Summary = "Get post likers (Paginated)",
        Description = "Retrieves a keyset paginated list of users who liked a specific post."
    )]
    public async Task<IActionResult> GetLikers(
        [FromRoute] Guid postId,
        [FromQuery] string? after,
        [FromQuery] int first = 20,
        CancellationToken cancellationToken = default)
    {
        Guid? currentUserId = null;
        if (User.Identity?.IsAuthenticated == true)
        {
            currentUserId = User.GetCurrentUserId();
        }

        var query = new GetPostLikersQuery
        {
            PostId = postId,
            CurrentUserId = currentUserId,
            After = after,
            First = first
        };
        var result = await _mediator.Send(query, cancellationToken);

        if (result == null)
            return NotFound(new { message = "Post not found" });
        return Ok(result);
    }
}
