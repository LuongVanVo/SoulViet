using MediatR;
using SoulViet.Modules.Social.Social.Application.Features.UserFollowers.Results;

namespace SoulViet.Modules.Social.Social.Application.Features.UserFollowers.Commands.FollowUser
{
    public class FollowUserCommand : IRequest<FollowerResponse>
    {
        public Guid FollowerId { get; set; }
        public Guid FollowingId { get; set; }
    }
}
