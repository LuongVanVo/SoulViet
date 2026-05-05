using Hangfire;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SoulViet.Modules.Social.Social.Application.Interfaces.Services;
using SoulViet.Shared.Application.Common.Events;

namespace SoulViet.Modules.Social.Social.Infrastructure.Consumer;

public class SocialQuestEventConsumer : 
    IConsumer<PostCreatedEvent>,
    IConsumer<PostDeletedEvent>,
    IConsumer<PostLikedEvent>,
    IConsumer<PostCommentedEvent>,
    IConsumer<PostSharedEvent>
{
    private readonly ISocialQuestService _socialQuestService;
    private readonly ILogger<SocialQuestEventConsumer> _logger;
    private readonly IBackgroundJobClient _backgroundJobClient;

    public SocialQuestEventConsumer(
        ISocialQuestService socialQuestService, 
        ILogger<SocialQuestEventConsumer> logger,
        IBackgroundJobClient backgroundJobClient)
    {
        _socialQuestService = socialQuestService;
        _logger = logger;
        _backgroundJobClient = backgroundJobClient;
    }

    public async Task Consume(ConsumeContext<PostCreatedEvent> context)
    {
        await _socialQuestService.ProcessCreatorRewardAsync(
            context.Message.PostId, 
            context.Message.UserId, 
            context.Message.HasVibeTag, 
            context.Message.HasCheckinLocation);
    }

    public async Task Consume(ConsumeContext<PostDeletedEvent> context)
    {
        await _socialQuestService.ProcessCreatorDeductionAsync(
            context.Message.PostId, 
            context.Message.UserId);
    }

    public async Task Consume(ConsumeContext<PostLikedEvent> context)
    {
        // Don't count self-like
        if (context.Message.ActorId == context.Message.PostOwnerId) return;

        await _socialQuestService.ProcessLikeQuestAsync(context.Message.PostId, context.Message.ActorId);

        // Check if the liked post is a pending share that needs validation
        await _socialQuestService.ValidateShareQuestAsync(context.Message.PostId, context.Message.PostOwnerId);
    }

    public async Task Consume(ConsumeContext<PostCommentedEvent> context)
    {
        // Don't count self-comment
        if (context.Message.ActorId == context.Message.PostOwnerId) return;

        await _socialQuestService.ProcessCommentQuestAsync(context.Message.PostId, context.Message.ActorId, context.Message.Content);

        // Check if the commented post is a pending share that needs validation
        await _socialQuestService.ValidateShareQuestAsync(context.Message.PostId, context.Message.PostOwnerId);
    }

    public async Task Consume(ConsumeContext<PostSharedEvent> context)
    {
        // Don't count self-share
        if (context.Message.ActorId == context.Message.PostOwnerId) return;

        await _socialQuestService.ProcessSharePendingAsync(context.Message.ShareId, context.Message.ActorId);

        // Schedule validation after 24 hours
        _backgroundJobClient.Schedule<ISocialQuestService>(
            service => service.ValidateShareQuestAsync(context.Message.ShareId, context.Message.ActorId), 
            TimeSpan.FromHours(24));
    }
}
