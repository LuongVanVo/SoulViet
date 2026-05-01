using SoulViet.Shared.Domain.Enums;
using SoulViet.Modules.Social.Social.Domain.Enums;
using SoulViet.Modules.Social.Social.Application.DTOs;

namespace SoulViet.Modules.Social.Social.Application.Features.Posts.Results;

public class PostResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? AuthorName { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Content { get; set; }
    public List<MediaItemResponse> Media { get; set; } = new();
    public List<Guid> TaggedProductIds { get; set; } = new();
    public VibeTag VibeTag { get; set; }
    public Guid? CheckinLocationId { get; set; }
    public string? CheckinLocationName { get; set; }
    public string? AspectRatio { get; set; }
    public int LikesCount { get; set; }
    public int CommentsCount { get; set; }
    public int SharesCount { get; set; }
    public bool IsLiked { get; set; }
    public bool IsFollowingAuthor { get; set; }
    public bool IsFollowerAuthor { get; set; }
    public PostStatus Status { get; set; }
    public PostResponse? OriginalPost { get; set; }
    public DateTime CreatedAt { get; set; }
}
