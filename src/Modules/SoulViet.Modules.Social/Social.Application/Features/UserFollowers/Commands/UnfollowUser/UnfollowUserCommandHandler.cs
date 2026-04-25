using MediatR;
using SoulViet.Modules.Social.Social.Application.Interfaces;
using SoulViet.Modules.Social.Social.Application.Interfaces.Services;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;
using SoulViet.Modules.Social.Social.Application.Features.UserFollowers.Results;
using SoulViet.Shared.Application.Interfaces;

namespace SoulViet.Modules.Social.Social.Application.Features.UserFollowers.Commands.UnfollowUser
{
    public class UnfollowUserCommandHandler : IRequestHandler<UnfollowUserCommand, FollowerResponse>
    {
        private readonly IUserFollowerRepository _followerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        public UnfollowUserCommandHandler(
            IUserFollowerRepository followerRepository,
            IUnitOfWork unitOfWork,
            ICacheService cacheService)
        {
            _followerRepository = followerRepository;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<FollowerResponse> Handle(UnfollowUserCommand request, CancellationToken cancellationToken)
        {
            var follow = await _followerRepository.GetAsync(request.FollowerId, request.FollowingId, cancellationToken);
            if (follow == null)
            {
                return new FollowerResponse { Success = true, Message = "Đã bỏ theo dõi người dùng này." };
            }

            _followerRepository.Remove(follow);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Update Cache
            var followingKey = $"user:{request.FollowerId}:following";
            var followersKey = $"user:{request.FollowingId}:followers";

            await _cacheService.ZRemoveAsync(followingKey, request.FollowingId, cancellationToken);
            await _cacheService.ZRemoveAsync(followersKey, request.FollowerId, cancellationToken);

            return new FollowerResponse { Success = true, Message = "Bỏ theo dõi thành công." };
        }
    }
}
