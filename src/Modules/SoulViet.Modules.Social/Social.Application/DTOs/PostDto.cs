using SoulViet.Shared.Domain.Enums;
using SoulViet.Modules.Social.Social.Domain.Enums;

namespace SoulViet.Modules.Social.Social.Application.DTOs;

public class PostDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public List<MediaItemDto> Media { get; set; } = new();
    public List<Guid> TaggedProductIds { get; set; } = new();
    public VibeTag VibeTag { get; set; }
    public Guid? CheckinLocationId { get; set; }
    public int LikesCount { get; set; }
    public int CommentsCount { get; set; }
    public int SharesCount { get; set; }
    public PostStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
}
