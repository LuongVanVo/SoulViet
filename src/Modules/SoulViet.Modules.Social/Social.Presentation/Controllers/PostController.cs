using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoulViet.Modules.Social.Social.Application.Features.PostComments.Queries.GetPostComments;
using SoulViet.Modules.Social.Social.Application.Features.Posts.Commands.CreatePost;
using SoulViet.Modules.Social.Social.Application.Features.Posts.Commands.DeletePost;
using SoulViet.Modules.Social.Social.Application.Features.Posts.Commands.UpdatePost;
using SoulViet.Modules.Social.Social.Application.Features.Posts.Queries.GetPostById;
using SoulViet.Modules.Social.Social.Application.Features.Posts.Queries.GetPostByUserId;
using SoulViet.Modules.Social.Social.Presentation.Helpers;
using Swashbuckle.AspNetCore.Annotations;

namespace SoulViet.Modules.Social.Social.Presentation.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class PostController : ControllerBase
{
    private readonly IMediator _mediator;

    public PostController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Create post",
        Description = "Creates a new social post for the current user."
    )]
    public async Task<IActionResult> Create(
        [FromBody] CreatePostCommand command,
        CancellationToken cancellationToken)
    {
        command.UserId = User.GetCurrentUserId();

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation(
        Summary = "Get post by id",
        Description = "Retrieves a specific post by its ID."
    )]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetPostByIdQuery
        {
            Id = id,
            UserId = User.GetCurrentUserId()
        };

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [SwaggerOperation(
        Summary = "Update post",
        Description = "Updates an existing post of the current user."
    )]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdatePostCommand command,
        CancellationToken cancellationToken)
    {
        command.Id = id;
        command.UserId = User.GetCurrentUserId();

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [SwaggerOperation(
        Summary = "Delete post",
        Description = "Deletes a specific post of the current user."
    )]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeletePostCommand
        {
            Id = id,
            UserId = User.GetCurrentUserId()
        };

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
    [HttpGet("{id:guid}/comments")]
    [AllowAnonymous] 
    [SwaggerOperation(
        Summary = "Get Post Comments (Paginated)",
        Description = "Retrieves a keyset paginated list of top-level comments for a specific post."
    )]
    public async Task<IActionResult> GetPostComments(
        [FromRoute] Guid id,
        [FromQuery] string? after,
        [FromQuery] int first = 20,
        [FromQuery] string sortBy = "newest",
        CancellationToken cancellationToken = default)
    {
        var query = new GetPostCommentsQuery
        {
            PostId = id,
            After = after,
            First = first,
            SortBy = string.IsNullOrWhiteSpace(sortBy) ? "newest" : sortBy.ToLowerInvariant()
        };
        var result = await _mediator.Send(query, cancellationToken);

        if (result == null)
            return NotFound(new { message = "Post not found" });
        return Ok(result);
    }

    [HttpGet("user/{userId:guid}")]
    [AllowAnonymous]
    [SwaggerOperation(
        Summary = "Get posts by user id (Paginated)",
        Description = "Retrieves a keyset paginated list of posts for a specific user."
    )]
    public async Task<IActionResult> GetByUserId(
        [FromRoute] Guid userId,
        [FromQuery] string? after,
        [FromQuery] int first = 20,
        [FromQuery] string sortBy = "newest",
        CancellationToken cancellationToken = default)
    {
        var query = new GetPostByUserIdQuery
        {
            UserId = userId,
            After = after,
            First = first,
            SortBy = string.IsNullOrWhiteSpace(sortBy) ? "newest" : sortBy.ToLowerInvariant()
        };
        var result = await _mediator.Send(query, cancellationToken);

        if (result == null)
            return NotFound(new { message = "User posts not found" });
        return Ok(result);
    }
}
