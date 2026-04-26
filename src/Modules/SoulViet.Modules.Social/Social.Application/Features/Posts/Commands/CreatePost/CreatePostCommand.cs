using MediatR;
using SoulViet.Modules.Social.Social.Application.Features.Posts.Commands;
using SoulViet.Modules.Social.Social.Application.Features.Posts.Results;
using SoulViet.Shared.Domain.Enums;

namespace SoulViet.Modules.Social.Social.Application.Features.Posts.Commands.CreatePost;

public class CreatePostCommand : IRequest<PostResponse>
{
    public Guid UserId { get; set; }
    public string? Content { get; set; }
    public List<MediaUploadRequest> Media { get; set; } = new();
    public List<Guid> TaggedProductIds { get; set; } = new();
    public VibeTag VibeTag { get; set; }
    public Guid? CheckinLocationId { get; set; }
    public string? AspectRatio { get; set; }
}

