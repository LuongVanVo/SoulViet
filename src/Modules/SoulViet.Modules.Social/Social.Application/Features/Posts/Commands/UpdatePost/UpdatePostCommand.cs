using MediatR;
using SoulViet.Modules.Social.Social.Application.Features.Posts.Results;
using SoulViet.Modules.Social.Social.Application.Features.Posts.Commands;
using SoulViet.Shared.Domain.Enums;

namespace SoulViet.Modules.Social.Social.Application.Features.Posts.Commands.UpdatePost;

public class UpdatePostCommand : IRequest<PostResponse>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? Content { get; set; }
    public List<MediaUploadRequest> Media { get; set; } = new();
    public List<Guid> TaggedProductIds { get; set; } = new();
    public VibeTag VibeTag { get; set; }
    public Guid? CheckinLocationId { get; set; }
    public string? AspectRatio { get; set; }
}

