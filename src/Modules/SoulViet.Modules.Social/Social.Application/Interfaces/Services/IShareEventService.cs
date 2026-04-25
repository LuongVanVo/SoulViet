namespace SoulViet.Modules.Social.Social.Application.Interfaces.Services;

public interface IShareEventService
{
    Task PublishShareChangedAsync(Guid postId, object sharePayload, CancellationToken cancellationToken = default);
    Task SubscribeAsync(Guid postId);
    Task UnsubscribeAsync(Guid postId);
}
