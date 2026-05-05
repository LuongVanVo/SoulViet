namespace SoulViet.Shared.Application.Common.Events;

public class PostDeletedEvent
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public DateTime DeletedAt { get; set; } = DateTime.UtcNow;
}
