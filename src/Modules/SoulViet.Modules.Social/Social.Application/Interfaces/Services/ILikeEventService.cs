namespace SoulViet.Modules.Social.Social.Application.Interfaces.Services;

public interface ILikeEventService
{
    Task PublishLikeChangedAsync(Guid postId, object likePayload, CancellationToken cancellationToken = default);
    Task SubscribeAsync(Guid postId);
    Task UnsubscribeAsync(Guid postId);
}
