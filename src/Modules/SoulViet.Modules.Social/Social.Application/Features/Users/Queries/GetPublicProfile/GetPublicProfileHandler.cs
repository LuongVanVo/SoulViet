using MediatR;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;
using SoulViet.Shared.Application.Exceptions;
using SoulViet.Shared.Application.Features.Auth.Results;
using SoulViet.Shared.Application.Interfaces.Repositories;

namespace SoulViet.Modules.Social.Social.Application.Features.Users.Queries.GetPublicProfile
{
    public class GetPublicProfileHandler : IRequestHandler<GetPublicProfileQuery, UserProfileResponse>
    {
        private readonly IUserRepository _identityUserRepository;
        private readonly IPostRepository _postRepository;
        private readonly IUserFollowerRepository _followerRepository;

        public GetPublicProfileHandler(
            IUserRepository identityUserRepository,
            IPostRepository postRepository,
            IUserFollowerRepository followerRepository)
        {
            _identityUserRepository = identityUserRepository;
            _postRepository = postRepository;
            _followerRepository = followerRepository;
        }

        public async Task<UserProfileResponse> Handle(GetPublicProfileQuery request, CancellationToken cancellationToken)
        {
            var user = await _identityUserRepository.GetUserByIdAsync(request.UserId);
            if (user == null)
                throw new NotFoundException("User not found.");

            var followersCount = await _followerRepository.GetFollowersCountAsync(user.Id, cancellationToken);
            var followingCount = await _followerRepository.GetFollowingCountAsync(user.Id, cancellationToken);
            var postsCount = await _postRepository.GetPostsCountByUserIdAsync(user.Id, cancellationToken);
            var roles = await _identityUserRepository.GetUserRolesAsync(user.Id);

            bool isFollowing = false;
            bool isFollower = false;

            if (request.CurrentUserId.HasValue && request.CurrentUserId.Value != user.Id)
            {
                isFollowing = await _followerRepository.ExistsAsync(request.CurrentUserId.Value, user.Id, cancellationToken);
                isFollower = await _followerRepository.ExistsAsync(user.Id, request.CurrentUserId.Value, cancellationToken);
            }

            return new UserProfileResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                AvatarUrl = user.AvatarUrl,
                Bio = user.Bio,
                FollowersCount = followersCount,
                FollowingCount = followingCount,
                PostsCount = postsCount,
                IsFollowing = isFollowing,
                IsFollower = isFollower,
                Roles = roles
            };
        }
    }
}
