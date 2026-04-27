using MediatR;
using SoulViet.Modules.Social.Social.Application.Features.UserFollowers.Results;

namespace SoulViet.Modules.Social.Social.Application.Features.UserFollowers.Commands.UnfollowUser
{
    public class UnfollowUserCommand : IRequest<FollowerResponse>
    {
        public Guid FollowerId { get; set; }
        public Guid FollowingId { get; set; }
    }
}
