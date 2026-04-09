using MediatR;
using SoulViet.Modules.Social.Social.Application.Features.Posts.Results;
using SoulViet.Shared.Domain.Enums;

namespace SoulViet.Modules.Social.Social.Application.Features.Posts.Commands.CreatePost;

public class CreatePostCommand : IRequest<PostResponse>
{
    public Guid UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public List<string> MediaUrls { get; set; } = new();
    public VibeTag VibeTag { get; set; }
    public Guid? CheckinLocationId { get; set; }
}

