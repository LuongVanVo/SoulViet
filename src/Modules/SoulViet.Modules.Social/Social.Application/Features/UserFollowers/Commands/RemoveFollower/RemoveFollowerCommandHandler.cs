using MediatR;
using SoulViet.Modules.Social.Social.Application.Interfaces;
using SoulViet.Modules.Social.Social.Application.Interfaces.Services;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;
using SoulViet.Modules.Social.Social.Application.Features.UserFollowers.Results;
using SoulViet.Shared.Application.Interfaces;

namespace SoulViet.Modules.Social.Social.Application.Features.UserFollowers.Commands.RemoveFollower
{
    public class RemoveFollowerCommandHandler : IRequestHandler<RemoveFollowerCommand, FollowerResponse>
    {
        private readonly IUserFollowerRepository _followerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        public RemoveFollowerCommandHandler(
            IUserFollowerRepository followerRepository,
            IUnitOfWork unitOfWork,
            ICacheService cacheService)
        {
            _followerRepository = followerRepository;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<FollowerResponse> Handle(RemoveFollowerCommand request, CancellationToken cancellationToken)
        {
            // Find the record where FollowerId is the user being removed, and FollowingId is the current user
            var follow = await _followerRepository.GetAsync(request.FollowerId, request.FollowingId, cancellationToken);
            if (follow == null)
            {
                return new FollowerResponse { Success = true, Message = "Người dùng này không còn theo dõi bạn." };
            }

            _followerRepository.Remove(follow);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Update Cache
            var followingKey = $"user:{request.FollowerId}:following";
            var followersKey = $"user:{request.FollowingId}:followers";

            await _cacheService.ZRemoveAsync(followingKey, request.FollowingId.ToString(), cancellationToken);
            await _cacheService.ZRemoveAsync(followersKey, request.FollowerId.ToString(), cancellationToken);

            return new FollowerResponse 
            { 
                Success = true, 
                Message = "Gỡ người theo dõi thành công.",
                IsFollowing = await _followerRepository.ExistsAsync(request.FollowingId, request.FollowerId, cancellationToken),
                IsFollower = false
            };
        }
    }
}
