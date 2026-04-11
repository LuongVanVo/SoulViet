using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoulViet.Modules.Social.Social.Application.Features.PostComments.Commands.CreatePostComment;
using SoulViet.Modules.Social.Social.Application.Features.PostComments.Commands.DeletePostComment;
using SoulViet.Modules.Social.Social.Application.Features.PostComments.Commands.UpdatePostComment;
using SoulViet.Modules.Social.Social.Application.Features.PostComments.Queries.GetPostCommentById;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoulViet.Modules.Social.Social.Presentation.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class PostCommentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PostCommentController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        [SwaggerOperation(
        Summary = "Create post comment",
        Description = "Creates a new comment for a social post."
        )]
        public async Task<IActionResult> CreatePostComment([FromBody] CreatePostCommentCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpGet("post/{postId:guid}")]
        [SwaggerOperation(
        Summary = "Get post comments by post id",
        Description = "Retrieves all comments with tree structure for a specific post."
        )]
        public async Task<IActionResult> GetPostCommentsByPostId([FromRoute] Guid postId, CancellationToken cancellationToken)
        {
            var query = new GetPostCommentByPostIdQuery { PostId = postId };
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        [SwaggerOperation(
            Summary = "Update post comment",
            Description = "Updates an existing post comment of the current post."
        )]
        public async Task<IActionResult> UpdatePostComment([FromRoute] Guid id, [FromBody] UpdatePostCommentCommand command, CancellationToken cancellationToken)
        {
            if (id != command.Id)
            {
                return BadRequest("ID in the route does not match ID in the body.");
            }
            var result = await _mediator.Send(command, cancellationToken);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpDelete("{id:guid}")]
        [SwaggerOperation(
            Summary = "Delete post comment",
            Description = "Deletes a specific post comment of the current post."
            )]
        public async Task<IActionResult> DeletePostComment([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var command = new DeletePostCommentCommand { Id = id };
            var result = await _mediator.Send(command, cancellationToken);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);

        }
    }
}
