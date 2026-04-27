using MediatR;
using SoulViet.Modules.Social.Social.Application.Common.Pagination;
using SoulViet.Modules.Social.Social.Application.Features.UserFollowers.Results;

namespace SoulViet.Modules.Social.Social.Application.Features.UserFollowers.Queries.GetFollowers;

public class GetFollowersQuery : IRequest<Connection<FollowerResponse>?>
{
    public Guid UserId { get; set; }
    public Guid CurrentUserId { get; set; }
    public string? After { get; set; }
    public int First { get; set; } = 20;
    public string SortBy { get; set; } = "newest";
}