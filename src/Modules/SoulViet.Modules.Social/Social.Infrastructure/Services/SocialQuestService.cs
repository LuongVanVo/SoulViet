using MassTransit;
using Microsoft.Extensions.Logging;
using SoulViet.Modules.Social.Social.Application.Interfaces.Services;
using SoulViet.Shared.Application.Common.Events;
using SoulViet.Shared.Application.Interfaces;

namespace SoulViet.Modules.Social.Social.Infrastructure.Services;

public class SocialQuestService : ISocialQuestService
{
    private readonly ICacheService _cacheService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<SocialQuestService> _logger;

    private static readonly int MaxCreatorRewardsPerDay = 3;
    private static readonly int QuestTarget = 10;
    
    private static readonly decimal RewardBasePost = 100m;
    private static readonly decimal RewardRichPost = 130m;
    private static readonly decimal RewardHighQualityPost = 150m;

    private static readonly decimal RewardLikeQuest = 30m;
    private static readonly decimal RewardCommentQuest = 50m;
    private static readonly decimal RewardShareQuest = 80m;

    public SocialQuestService(ICacheService cacheService, IPublishEndpoint publishEndpoint, ILogger<SocialQuestService> logger)
    {
        _cacheService = cacheService;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    private string GetTodayString() => DateTime.UtcNow.ToString("yyyy-MM-dd");

    public async Task ProcessCreatorRewardAsync(Guid postId, Guid userId, bool hasVibeTag, bool hasCheckinLocation)
    {
        var today = GetTodayString();
        var dailyCountKey = $"quest:creator:count:{userId}:{today}";
        
        var currentCount = await _cacheService.GetAsync<int?>(dailyCountKey) ?? 0;
        if (currentCount >= MaxCreatorRewardsPerDay)
        {
            _logger.LogInformation("User {UserId} reached max creator rewards for today.", userId);
            return;
        }

        decimal rewardAmount = RewardBasePost;
        if (hasVibeTag && hasCheckinLocation) rewardAmount = RewardHighQualityPost;
        else if (hasVibeTag || hasCheckinLocation) rewardAmount = RewardRichPost;

        // Save awarded amount so we can deduct exactly this amount if deleted
        await _cacheService.SetAsync($"quest:creator:reward:{postId}", rewardAmount, TimeSpan.FromDays(30));
        
        await _cacheService.SetAsync(dailyCountKey, currentCount + 1, TimeSpan.FromDays(1));

        await _publishEndpoint.Publish(new SoulCoinEarnedEvent
        {
            UserId = userId,
            Amount = rewardAmount,
            Description = $"Thưởng đăng bài viết mới",
            ReferenceId = postId.ToString()
        });
    }

    public async Task ProcessCreatorDeductionAsync(Guid postId, Guid userId)
    {
        var rewardKey = $"quest:creator:reward:{postId}";
        var rewardedAmount = await _cacheService.GetAsync<decimal?>(rewardKey);

        if (rewardedAmount.HasValue)
        {
            await _publishEndpoint.Publish(new SoulCoinDeductedEvent
            {
                UserId = userId,
                Amount = rewardedAmount.Value,
                Description = $"Thu hồi điểm do xóa bài viết",
                ReferenceId = postId.ToString()
            });
            await _cacheService.RemoveAsync(rewardKey);
        }

        // Cancel pending share if any
        var pendingShareKey = $"quest:share:pending:{postId}";
        await _cacheService.RemoveAsync(pendingShareKey);
    }

    public async Task ProcessLikeQuestAsync(Guid postId, Guid userId)
    {
        await ProcessInteractionQuestAsync(userId, postId, "like", RewardLikeQuest, "Hoàn thành nhiệm vụ Like 10 bài");
    }

    public async Task ProcessCommentQuestAsync(Guid postId, Guid userId, string content)
    {
        if (string.IsNullOrWhiteSpace(content) || content.Length < 50)
        {
            return;
        }
        await ProcessInteractionQuestAsync(userId, postId, "comment", RewardCommentQuest, "Hoàn thành nhiệm vụ Bình luận 10 bài");
    }

    public async Task ProcessSharePendingAsync(Guid shareId, Guid actorId)
    {
        // Store pending share status. Will be validated by Hangfire job or interaction.
        var pendingKey = $"quest:share:pending:{shareId}";
        await _cacheService.SetAsync(pendingKey, actorId, TimeSpan.FromHours(25));
    }

    public async Task ValidateShareQuestAsync(Guid shareId, Guid actorId)
    {
        var pendingKey = $"quest:share:pending:{shareId}";
        var exists = await _cacheService.GetAsync<Guid?>(pendingKey);
        
        if (exists != null && exists.Value == actorId)
        {
            await _cacheService.RemoveAsync(pendingKey);
            await ProcessInteractionQuestAsync(actorId, shareId, "share", RewardShareQuest, "Hoàn thành nhiệm vụ Chia sẻ 10 bài");
        }
    }

    private async Task ProcessInteractionQuestAsync(Guid userId, Guid postId, string questType, decimal rewardAmount, string description)
    {
        var today = GetTodayString();
        var rewardedKey = $"quest:{questType}:rewarded:{userId}:{today}";
        
        var isRewarded = await _cacheService.GetAsync<bool?>(rewardedKey);
        if (isRewarded == true) return; // Already completed this quest today

        var setKey = $"quest:{questType}:set:{userId}:{today}";
        var interactedPosts = await _cacheService.GetAsync<List<Guid>>(setKey) ?? new List<Guid>();

        if (!interactedPosts.Contains(postId))
        {
            interactedPosts.Add(postId);
            await _cacheService.SetAsync(setKey, interactedPosts, TimeSpan.FromDays(1));

            if (interactedPosts.Count >= QuestTarget)
            {
                await _cacheService.SetAsync(rewardedKey, true, TimeSpan.FromDays(1));
                await _publishEndpoint.Publish(new SoulCoinEarnedEvent
                {
                    UserId = userId,
                    Amount = rewardAmount,
                    Description = description,
                    ReferenceId = $"{questType}_{today}"
                });
            }
        }
    }

    public async Task<SoulViet.Modules.Social.Social.Application.DTOs.DailyQuestProgressDto> GetDailyQuestProgressAsync(Guid userId)
    {
        var today = GetTodayString();
        
        var likeSet = await _cacheService.GetAsync<List<Guid>>($"quest:like:set:{userId}:{today}") ?? new List<Guid>();
        var likeRewarded = await _cacheService.GetAsync<bool?>($"quest:like:rewarded:{userId}:{today}") ?? false;

        var commentSet = await _cacheService.GetAsync<List<Guid>>($"quest:comment:set:{userId}:{today}") ?? new List<Guid>();
        var commentRewarded = await _cacheService.GetAsync<bool?>($"quest:comment:rewarded:{userId}:{today}") ?? false;

        var shareSet = await _cacheService.GetAsync<List<Guid>>($"quest:share:set:{userId}:{today}") ?? new List<Guid>();
        var shareRewarded = await _cacheService.GetAsync<bool?>($"quest:share:rewarded:{userId}:{today}") ?? false;

        var creatorCount = await _cacheService.GetAsync<int?>($"quest:creator:count:{userId}:{today}") ?? 0;

        return new SoulViet.Modules.Social.Social.Application.DTOs.DailyQuestProgressDto
        {
            LikeQuest = new SoulViet.Modules.Social.Social.Application.DTOs.QuestProgressItem 
            { 
                Current = likeSet.Count, 
                IsRewarded = likeRewarded 
            },
            CommentQuest = new SoulViet.Modules.Social.Social.Application.DTOs.QuestProgressItem 
            { 
                Current = commentSet.Count, 
                IsRewarded = commentRewarded 
            },
            ShareQuest = new SoulViet.Modules.Social.Social.Application.DTOs.QuestProgressItem 
            { 
                Current = shareSet.Count, 
                IsRewarded = shareRewarded 
            },
            CreatorPostsCount = creatorCount
        };
    }
}
