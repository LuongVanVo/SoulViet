using SoulViet.Shared.Domain.Enums;

namespace SoulViet.Modules.Social.Social.Application.Features.Posts.Results;

public class PostResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public List<string> MediaUrls { get; set; } = new();
    public List<Guid> TaggedProductIds { get; set; } = new();
    public VibeTag VibeTag { get; set; }
    public Guid? CheckinLocationId { get; set; }
    public int LikesCount { get; set; }
    public int CommentsCount { get; set; }
    public int SharesCount { get; set; }
    public PostStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
