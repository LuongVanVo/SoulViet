using MediatR;
using SoulViet.Shared.Application.Common.Events;
using SoulViet.Shared.Application.Exceptions;
using SoulViet.Shared.Application.Interfaces;
using SoulViet.Modules.Social.Social.Application.Interfaces;
using SoulViet.Modules.Social.Social.Application.Interfaces.Services;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;
using SoulViet.Modules.Social.Social.Domain.Entities;
using SoulViet.Modules.Social.Social.Application.Features.PostLikes.Results;

namespace SoulViet.Modules.Social.Social.Application.Features.PostLikes.Commands.Like
{
    public record LikePostCommand(Guid PostId, Guid UserId, string UserName) : IRequest<PostLikeResult>;

    public class LikePostCommandHandler : IRequestHandler<LikePostCommand, PostLikeResult>
    {
        private readonly IPostRepository _postRepository;
        private readonly IPostLikeRepository _postLikeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;
        private readonly ILikeEventService _likeEventService;

        private static string LikesKey(Guid postId) => $"post:likes:{postId}";

        public LikePostCommandHandler(
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

        public async Task<PostLikeResult> Handle(LikePostCommand request, CancellationToken cancellationToken)
        {
            var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
            if (post == null)
                throw new NotFoundException("Post not found");

            var existingLike = await _postLikeRepository.GetPostLikeAsync(request.PostId, request.UserId, cancellationToken);
            if (existingLike != null)
            {
                var cached = await _cacheService.GetAsync<long?>(LikesKey(request.PostId), cancellationToken);
                return new PostLikeResult(true, (int)(cached ?? post.LikesCount), request.PostId);
            }

            var postLike = new PostLike
            {
                PostId = request.PostId,
                UserId = request.UserId,
                CreatedAt = DateTime.UtcNow
            };
            await _postLikeRepository.AddAsync(postLike, cancellationToken);

            var redisKey = LikesKey(request.PostId);
            var existsInRedis = await _cacheService.GetAsync<long?>(redisKey, cancellationToken);
            if (existsInRedis == null)
                await _cacheService.SetAsync(redisKey, (long)post.LikesCount, cancellationToken: cancellationToken);

            var newCount = await _cacheService.IncrementAsync(redisKey, cancellationToken);

            post.LikesCount = (int)newCount;
            _postRepository.Update(post);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = new PostLikeResult(true, (int)newCount, request.PostId);
            
            await _likeEventService.PublishLikeChangedAsync(request.PostId, result, cancellationToken);

            return result;
        }
    }
}