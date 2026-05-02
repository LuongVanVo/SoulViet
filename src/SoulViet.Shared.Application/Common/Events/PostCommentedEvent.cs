using System;

namespace SoulViet.Shared.Application.Common.Events;

public class PostCommentedEvent
{
    public Guid PostId { get; set; }
    public Guid PostOwnerId { get; set; }
    public Guid ActorId { get; set; }
    public string ActorName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
