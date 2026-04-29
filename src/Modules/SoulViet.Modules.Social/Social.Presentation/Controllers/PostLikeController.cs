using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoulViet.Modules.Social.Social.Application.Features.PostLikes.Commands.Like;
using SoulViet.Modules.Social.Social.Application.Features.PostLikes.Commands.Unlike;
using SoulViet.Modules.Social.Social.Presentation.Helpers;
using Swashbuckle.AspNetCore.Annotations;

namespace SoulViet.Modules.Social.Social.Presentation.Controllers;

[ApiController]
[Route("api/posts")]
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
        var userName = User.Identity?.Name ?? "Anonymous"; 
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
}
