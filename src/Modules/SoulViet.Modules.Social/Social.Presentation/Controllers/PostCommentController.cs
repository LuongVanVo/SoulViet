using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SoulViet.Modules.Social.Social.Application.Features.PostComments.Commands.CreatePostComment;
using SoulViet.Modules.Social.Social.Application.Features.PostComments.Commands.DeletePostComment;
using SoulViet.Modules.Social.Social.Application.Features.PostComments.Commands.UpdatePostComment;
using SoulViet.Modules.Social.Social.Application.Features.PostComments.Queries.GetPostCommentById;
using SoulViet.Modules.Social.Social.Infrastructure.Services;
using SoulViet.Modules.Social.Social.Presentation.Helpers;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using SoulViet.Modules.Social.Social.Application.Interfaces.Services;

namespace SoulViet.Modules.Social.Social.Presentation.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class PostCommentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly SseConnectionManager _sseManager;
        private readonly int _sseIdleTimeoutSeconds;
        private readonly ICommentEventService _commentEventService;
        private readonly SoulViet.Shared.Application.Interfaces.ICacheService _cacheService;
        private readonly ILogger<PostCommentController> _logger;

        public PostCommentController(
            IMediator mediator, 
            SseConnectionManager sseManager, 
            IConfiguration configuration,
            ICommentEventService commentEventService,
            SoulViet.Shared.Application.Interfaces.ICacheService cacheService,
            ILogger<PostCommentController> logger)
        {
            _mediator = mediator;
            _sseManager = sseManager;
            _sseIdleTimeoutSeconds = configuration.GetValue<int>("Sse:IdleTimeoutSeconds", 60);
            _commentEventService = commentEventService;
            _cacheService = cacheService;
            _logger = logger;
        }
        [HttpPost]
        [SwaggerOperation(
        Summary = "Create post comment",
        Description = "Creates a new comment for a social post."
        )]
        public async Task<IActionResult> CreatePostComment([FromBody] CreatePostCommentCommand command, CancellationToken cancellationToken)
        {
            command.UserId = User.GetCurrentUserId();
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
            command.UserId = User.GetCurrentUserId();
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
            var command = new DeletePostCommentCommand { Id = id, UserId = User.GetCurrentUserId() };
            var result = await _mediator.Send(command, cancellationToken);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);

        }

        [HttpGet("/api/comments/{commentId:guid}/replies")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Get comment replies (Paginated)",
            Description = "Retrieves a keyset paginated list of nested replies for a given comment."
        )]
        public async Task<IActionResult> GetCommentReplies(
            [FromRoute] Guid commentId,
            [FromQuery] string? after,
            [FromQuery] int first = 20,
            [FromQuery] string sortBy = "newest",
            CancellationToken cancellationToken = default)
        {
            var query = new SoulViet.Modules.Social.Social.Application.Features.PostComments.Queries.GetCommentReplies.GetCommentRepliesQuery
            {
                CommentId = commentId,
                After = after,
                First = first,
                SortBy = string.IsNullOrWhiteSpace(sortBy) ? "newest" : sortBy.ToLowerInvariant()
            };

            var result = await _mediator.Send(query, cancellationToken);

            if (result == null)
                return NotFound(new { message = "Parent comment not found" });

            return Ok(result);
        }

        [HttpGet("stream")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "SSE comment stream",
            Description = "Subscribe to real-time comment events for a post via Server-Sent Events."
        )]
        public async Task StreamComments([FromQuery] Guid postId, CancellationToken cancellationToken)
        {
            SseWriter.SetSseHeaders(Response);

            using var idleCts = new CancellationTokenSource(TimeSpan.FromSeconds(_sseIdleTimeoutSeconds));
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, idleCts.Token);

            var client = new SseClient(Guid.NewGuid(), Response, linkedCts.Token);
            _sseManager.AddClient(postId, client);
            await _commentEventService.SubscribeAsync(postId);

            try
            {
                await SseWriter.WriteKeepAliveAsync(Response);

                // Fetch initial comments count
                int initialCount = 0;
                try
                {
                    var query = new SoulViet.Modules.Social.Social.Application.Features.Posts.Queries.GetPostById.GetPostByIdQuery { Id = postId };
                    var post = await _mediator.Send(query, cancellationToken);
                    initialCount = post?.CommentsCount ?? 0;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "[SSE:Comments] Failed to fetch initial comment count for postId={PostId}. Defaulting to 0.", postId);
                    initialCount = 0;
                }

                var initialPayload = System.Text.Json.JsonSerializer.Serialize(new { 
                    success = true, 
                    commentsCount = initialCount, 
                    postId = postId 
                });
                var initialId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
                await SseWriter.WriteEventAsync(Response, "comment", initialPayload, initialId);

                await Task.Delay(Timeout.Infinite, linkedCts.Token);
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                _sseManager.RemoveClient(postId, client);
                await _commentEventService.UnsubscribeAsync(postId);
            }
        }
    }
}
