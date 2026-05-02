using MediatR;
using SoulViet.Shared.Application.Exceptions;
using SoulViet.Shared.Application.Interfaces;
using SoulViet.Modules.Social.Social.Application.Interfaces;
using SoulViet.Modules.Social.Social.Application.Interfaces.Services;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;
using SoulViet.Modules.Social.Social.Application.Features.PostLikes.Results;

namespace SoulViet.Modules.Social.Social.Application.Features.PostLikes.Commands.Unlike
{
    public record UnlikePostCommand(Guid PostId, Guid UserId) : IRequest<PostLikeResult>;

    public class UnlikePostCommandHandler : IRequestHandler<UnlikePostCommand, PostLikeResult>
    {
        private readonly IPostRepository _postRepository;
        private readonly IPostLikeRepository _postLikeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        private static string LikesKey(Guid postId) => $"post:likes:{postId}";

        public UnlikePostCommandHandler(
            IPostRepository postRepository,
            IPostLikeRepository postLikeRepository,
            IUnitOfWork unitOfWork,
            ICacheService cacheService)
        {
            _postRepository = postRepository;
            _postLikeRepository = postLikeRepository;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<PostLikeResult> Handle(UnlikePostCommand request, CancellationToken cancellationToken)
        {
            var existingLike = await _postLikeRepository.GetPostLikeAsync(request.PostId, request.UserId, cancellationToken);
            if (existingLike == null)
            {
                var cachedCount = await _cacheService.GetAsync<long?>(LikesKey(request.PostId), cancellationToken);
                if (!cachedCount.HasValue)
                {
                    var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
                    cachedCount = post?.LikesCount ?? 0;
                }
                return new PostLikeResult(false, (int)cachedCount.Value, request.PostId, request.UserId);
            }

            try 
            {
                _postLikeRepository.Remove(existingLike);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _postRepository.DecrementLikesCountAsync(request.PostId, cancellationToken);

                var redisKey = LikesKey(request.PostId);
                var newCount = await _cacheService.DecrementAsync(redisKey, cancellationToken);

                var result = new PostLikeResult(false, (int)newCount, request.PostId, request.UserId);
                
                return result;
            }
            catch (Exception)
            {
                var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
                return new PostLikeResult(false, post?.LikesCount ?? 0, request.PostId, request.UserId);
            }
        }
    }
}