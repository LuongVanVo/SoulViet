using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SoulViet.Modules.Social.Social.Application.Features.PostLikes.Commands.Like;
using SoulViet.Modules.Social.Social.Application.Features.PostLikes.Commands.Unlike;
using SoulViet.Modules.Social.Social.Application.Interfaces.Services;
using SoulViet.Modules.Social.Social.Infrastructure.Services;
using SoulViet.Modules.Social.Social.Presentation.Helpers;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.Extensions.Logging;

namespace SoulViet.Modules.Social.Social.Presentation.Controllers;

[ApiController]
[Route("api/posts/{postId}/likes")]
[Authorize]
public class PostLikeController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly SseConnectionManager _sseManager;
    private readonly int _sseIdleTimeoutSeconds;
    private readonly ILikeEventService _likeEventService;
    private readonly SoulViet.Shared.Application.Interfaces.ICacheService _cacheService;
    private readonly ILogger<PostLikeController> _logger;

    public PostLikeController(
        IMediator mediator, 
        SseConnectionManager sseManager, 
        IConfiguration configuration,
        ILikeEventService likeEventService,
        SoulViet.Shared.Application.Interfaces.ICacheService cacheService,
        ILogger<PostLikeController> logger)
    {
        _mediator = mediator;
        _sseManager = sseManager;
        _sseIdleTimeoutSeconds = configuration.GetValue<int>("Sse:IdleTimeoutSeconds", 60);
        _likeEventService = likeEventService;
        _cacheService = cacheService;
        _logger = logger;
    }

    [HttpPost]
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

    [HttpDelete]
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

    [HttpGet("stream")]
    [AllowAnonymous]
    [SwaggerOperation(
        Summary = "SSE like stream",
        Description = "Subscribe to real-time like events for a post via Server-Sent Events."
    )]
    public async Task StreamLikes([FromRoute] Guid postId, CancellationToken cancellationToken)
    {
        SseWriter.SetSseHeaders(Response);

        using var idleCts = new CancellationTokenSource(TimeSpan.FromSeconds(_sseIdleTimeoutSeconds));
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, idleCts.Token);

        var client = new SseClient(Guid.NewGuid(), Response, linkedCts.Token);
        _sseManager.AddClient(postId, client);
        await _likeEventService.SubscribeAsync(postId);

        try
        {
            await SseWriter.WriteKeepAliveAsync(Response);

            var redisKey = $"post:likes:{postId}";
            var cachedCount = await _cacheService.GetAsync<long?>(redisKey, cancellationToken);
            
            int initialCount;
            if (cachedCount != null)
            {
                initialCount = (int)cachedCount.Value;
            }
            else
            {
                try
                {
                    var query = new SoulViet.Modules.Social.Social.Application.Features.Posts.Queries.GetPostById.GetPostByIdQuery { Id = postId };
                    var post = await _mediator.Send(query, cancellationToken);
                    initialCount = post?.LikesCount ?? 0;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "[SSE:Likes] Failed to fetch initial like count for postId={PostId}. Defaulting to 0.", postId);
                    initialCount = 0;
                }
            }

            var initialPayload = System.Text.Json.JsonSerializer.Serialize(new { 
                success = true, 
                likesCount = initialCount, 
                postId = postId 
            });
            var initialId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            await SseWriter.WriteEventAsync(Response, "like", initialPayload, initialId);

            await Task.Delay(Timeout.Infinite, linkedCts.Token);
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            _sseManager.RemoveClient(postId, client);
            await _likeEventService.UnsubscribeAsync(postId);
        }
    }
}
