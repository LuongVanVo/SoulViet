using SoulViet.Shared.Domain.Enums;

namespace SoulViet.Shared.Application.Common.Events;

public class PostSharedEvent
{
    public Guid PostId { get; set; }
    public Guid ShareId { get; set; }
    public Guid PostOwnerId { get; set; }
    public Guid ActorId { get; set; }
    public string ActorName { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;
    public ShareType ShareType { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
