using MediatR;
using SoulViet.Modules.Social.Social.Application.Features.UserFollowers.Results;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;
using SoulViet.Shared.Application.Interfaces.Repositories;

namespace SoulViet.Modules.Social.Social.Application.Features.UserFollowers.Queries.GetFollowStatus
{
    public class GetFollowStatusHandler : IRequestHandler<GetFollowStatusQuery, FollowerResponse>
    {
        private readonly IUserFollowerRepository _followerRepository;
        private readonly IUserRepository _userRepository;

        public GetFollowStatusHandler(IUserFollowerRepository followerRepository, IUserRepository userRepository)
        {
            _followerRepository = followerRepository;
            _userRepository = userRepository;
        }

        public async Task<FollowerResponse> Handle(GetFollowStatusQuery request, CancellationToken cancellationToken)
        {
            var isFollowing = await _followerRepository.ExistsAsync(request.FollowerId, request.FollowingId, cancellationToken);
            var isFollower = await _followerRepository.ExistsAsync(request.FollowingId, request.FollowerId, cancellationToken);
            var followingUser = await _userRepository.GetUserByIdAsync(request.FollowingId);

            return new FollowerResponse
            {
                Success = true,
                Message = "Follow status retrieved successfully.",
                UserId = request.FollowingId,
                FullName = followingUser?.FullName ?? string.Empty,
                AvatarUrl = followingUser?.AvatarUrl,
                IsFollowing = isFollowing,
                IsFollower = isFollower
            };
        }
    }
}
