using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SoulViet.Modules.Social.Social.Application.Features.PostShares.Commands.Share;
using SoulViet.Modules.Social.Social.Application.Interfaces.Services;
using SoulViet.Modules.Social.Social.Infrastructure.Services;
using SoulViet.Modules.Social.Social.Presentation.Helpers;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.Extensions.Logging;

using SoulViet.Modules.Social.Social.Application.DTOs;

namespace SoulViet.Modules.Social.Social.Presentation.Controllers;

[ApiController]
[Route("api/posts/{postId}/shares")]
[Authorize]
public class PostShareController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly SseConnectionManager _sseManager;
    private readonly int _sseIdleTimeoutSeconds;
    private readonly IShareEventService _shareEventService;
    private readonly SoulViet.Shared.Application.Interfaces.ICacheService _cacheService;
    private readonly ILogger<PostShareController> _logger;

    public PostShareController(
        IMediator mediator, 
        SseConnectionManager sseManager, 
        IConfiguration configuration,
        IShareEventService shareEventService,
        SoulViet.Shared.Application.Interfaces.ICacheService cacheService,
        ILogger<PostShareController> logger)
    {
        _mediator = mediator;
        _sseManager = sseManager;
        _sseIdleTimeoutSeconds = configuration.GetValue<int>("Sse:IdleTimeoutSeconds", 60);
        _shareEventService = shareEventService;
        _cacheService = cacheService;
        _logger = logger;
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Share a post",
        Description = "Shares a post. Returns a share link."
    )]
    public async Task<IActionResult> Share(
        [FromRoute] Guid postId,
        [FromBody] PostShareDto requestDto,
        CancellationToken cancellationToken)
    {
        var userName = User.Identity?.Name ?? "Anonymous"; 
        
        var command = new PostShareCommand(
            postId, 
            User.GetCurrentUserId(), 
            userName, 
            requestDto.Caption ?? string.Empty,
            requestDto.ShareType
        );
        
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpGet("stream")]
    [AllowAnonymous]
    [SwaggerOperation(
        Summary = "SSE share stream",
        Description = "Subscribe to real-time share events for a post via Server-Sent Events."
    )]
    public async Task StreamShares([FromRoute] Guid postId, CancellationToken cancellationToken)
    {
        SseWriter.SetSseHeaders(Response);

        using var idleCts = new CancellationTokenSource(TimeSpan.FromSeconds(_sseIdleTimeoutSeconds));
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, idleCts.Token);

        var client = new SseClient(Guid.NewGuid(), Response, linkedCts.Token);
        _sseManager.AddClient(postId, client);
        await _shareEventService.SubscribeAsync(postId);

        try
        {
            await SseWriter.WriteKeepAliveAsync(Response);

            var redisKey = $"post:shares:{postId}";
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
                    initialCount = post?.SharesCount ?? 0;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "[SSE:Shares] Failed to fetch initial share count for postId={PostId}. Defaulting to 0.", postId);
                    initialCount = 0;
                }
            }

            var initialPayload = System.Text.Json.JsonSerializer.Serialize(new { 
                success = true, 
                sharesCount = initialCount, 
                postId = postId 
            });
            var initialId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            await SseWriter.WriteEventAsync(Response, "share", initialPayload, initialId);

            await Task.Delay(Timeout.Infinite, linkedCts.Token);
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            _sseManager.RemoveClient(postId, client);
            await _shareEventService.UnsubscribeAsync(postId);
        }
    }
}
