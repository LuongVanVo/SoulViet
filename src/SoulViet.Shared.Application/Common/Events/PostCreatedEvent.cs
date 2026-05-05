using System;

namespace SoulViet.Shared.Application.Common.Events;

public class PostCreatedEvent
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public bool HasVibeTag { get; set; }
    public bool HasCheckinLocation { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
