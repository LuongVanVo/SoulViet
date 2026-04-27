namespace SoulViet.Shared.Application.Common.Events;

public class UserFollowedEvent
{
    public Guid FollowerId { get; set; }
    public string FollowerName { get; set; } = string.Empty;
    public Guid FollowingId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
