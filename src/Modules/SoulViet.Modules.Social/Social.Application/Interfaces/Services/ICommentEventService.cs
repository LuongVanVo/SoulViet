namespace SoulViet.Modules.Social.Social.Application.Interfaces.Services;

public interface ICommentEventService
{
    Task PublishCommentAsync(Guid postId, object commentPayload, CancellationToken cancellationToken = default);
    Task SubscribeAsync(Guid postId);
    Task UnsubscribeAsync(Guid postId);
}
