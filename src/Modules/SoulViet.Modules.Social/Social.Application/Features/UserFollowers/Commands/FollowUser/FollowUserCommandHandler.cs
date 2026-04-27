using MassTransit;
using MediatR;
using SoulViet.Modules.Social.Social.Application.Interfaces;
using SoulViet.Modules.Social.Social.Application.Interfaces.Services;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;
using SoulViet.Modules.Social.Social.Domain.Entities;
using SoulViet.Shared.Application.Common.Events;
using SoulViet.Shared.Application.Interfaces;
using SoulViet.Modules.Social.Social.Application.Features.UserFollowers.Results;

namespace SoulViet.Modules.Social.Social.Application.Features.UserFollowers.Commands.FollowUser
{
    public class FollowUserCommandHandler : IRequestHandler<FollowUserCommand, FollowerResponse>
    {
        private readonly IUserFollowerRepository _followerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IUserService _userService;

        public FollowUserCommandHandler(
            IUserFollowerRepository followerRepository,
            IUnitOfWork unitOfWork,
            ICacheService cacheService,
            IPublishEndpoint publishEndpoint,
            IUserService userService)
        {
            _followerRepository = followerRepository;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _publishEndpoint = publishEndpoint;
            _userService = userService;
        }

        public async Task<FollowerResponse> Handle(FollowUserCommand request, CancellationToken cancellationToken)
        {
            if (request.FollowerId == request.FollowingId)
            {
                return new FollowerResponse { Success = false, Message = "Bạn không thể tự theo dõi chính mình." };
            }

            var isFollowing = await _followerRepository.ExistsAsync(request.FollowerId, request.FollowingId, cancellationToken);
            if (isFollowing)
            {
                return new FollowerResponse { Success = true, Message = "Đã theo dõi người dùng này." };
            }

            var follow = new UserFollower
            {
                FollowerId = request.FollowerId,
                FollowingId = request.FollowingId,
                CreatedAt = DateTime.UtcNow
            };

            await _followerRepository.AddAsync(follow, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Update Cache: Redis Sorted Sets
            var followingKey = $"user:{request.FollowerId}:following";
            var followersKey = $"user:{request.FollowingId}:followers";

            var score = ((DateTimeOffset)follow.CreatedAt).ToUnixTimeSeconds();

            await _cacheService.ZAddAsync(followingKey, request.FollowingId, score, cancellationToken);
            await _cacheService.ZAddAsync(followersKey, request.FollowerId, score, cancellationToken);

            var users = await _userService.GetUsersMinimalInfoAsync(new[] { request.FollowerId }, cancellationToken);
            var followerName = users.ContainsKey(request.FollowerId) ? users[request.FollowerId].FullName : "Ai đó";

            await _publishEndpoint.Publish(new UserFollowedEvent
            {
                FollowerId = request.FollowerId,
                FollowerName = followerName,
                FollowingId = request.FollowingId,
                CreatedAt = follow.CreatedAt
            }, cancellationToken);

            return new FollowerResponse { Success = true, Message = "Theo dõi thành công." };
        }
    }
}
