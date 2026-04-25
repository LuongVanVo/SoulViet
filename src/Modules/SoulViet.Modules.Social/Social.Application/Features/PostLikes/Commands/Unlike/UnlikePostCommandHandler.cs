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
        private readonly ILikeEventService _likeEventService;

        private static string LikesKey(Guid postId) => $"post:likes:{postId}";

        public UnlikePostCommandHandler(
            IPostRepository postRepository,
            IPostLikeRepository postLikeRepository,
            IUnitOfWork unitOfWork,
            ICacheService cacheService,
            ILikeEventService likeEventService)
        {
            _postRepository = postRepository;
            _postLikeRepository = postLikeRepository;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _likeEventService = likeEventService;
        }

        public async Task<PostLikeResult> Handle(UnlikePostCommand request, CancellationToken cancellationToken)
        {
            var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
            if (post == null)
                throw new NotFoundException("Post not found");

            var existingLike = await _postLikeRepository.GetPostLikeAsync(request.PostId, request.UserId, cancellationToken);
            if (existingLike == null)
            {
                var cached = await _cacheService.GetAsync<long?>(LikesKey(request.PostId), cancellationToken);
                return new PostLikeResult(false, (int)(cached ?? post.LikesCount), request.PostId);
            }

            _postLikeRepository.Remove(existingLike);

            var redisKey = LikesKey(request.PostId);
            var existsInRedis = await _cacheService.GetAsync<long?>(redisKey, cancellationToken);
            if (existsInRedis == null)
                await _cacheService.SetAsync(redisKey, (long)post.LikesCount, cancellationToken: cancellationToken);

            var newCount = await _cacheService.DecrementAsync(redisKey, cancellationToken);

            post.LikesCount = (int)newCount;
            _postRepository.Update(post);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = new PostLikeResult(false, (int)newCount, request.PostId);
            await _likeEventService.PublishLikeChangedAsync(request.PostId, result, cancellationToken);

            return result;
        }
    }
}