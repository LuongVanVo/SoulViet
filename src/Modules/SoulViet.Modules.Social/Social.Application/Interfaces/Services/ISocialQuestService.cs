namespace SoulViet.Modules.Social.Social.Application.Interfaces.Services;

public interface ISocialQuestService
{
    Task ProcessCreatorRewardAsync(Guid postId, Guid userId, bool hasVibeTag, bool hasCheckinLocation);
    Task ProcessCreatorDeductionAsync(Guid postId, Guid userId);
    Task ProcessLikeQuestAsync(Guid postId, Guid userId);
    Task ProcessCommentQuestAsync(Guid postId, Guid userId, string content);
    Task ProcessSharePendingAsync(Guid shareId, Guid actorId);
    Task ValidateShareQuestAsync(Guid shareId, Guid actorId);
    Task<SoulViet.Modules.Social.Social.Application.DTOs.DailyQuestProgressDto> GetDailyQuestProgressAsync(Guid userId);
}
