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
        private readonly MassTransit.IPublishEndpoint _publishEndpoint;

        private static string LikesKey(Guid postId) => $"post:likes:{postId}";

        public LikePostCommandHandler(
            IPostRepository postRepository,
            IPostLikeRepository postLikeRepository,
            IUnitOfWork unitOfWork,
            ICacheService cacheService,
            MassTransit.IPublishEndpoint publishEndpoint)
        {
            _postRepository = postRepository;
            _postLikeRepository = postLikeRepository;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<PostLikeResult> Handle(LikePostCommand request, CancellationToken cancellationToken)
        {
            var isAlreadyLiked = await _postLikeRepository.GetPostLikeAsync(request.PostId, request.UserId, cancellationToken) != null;
            
            if (isAlreadyLiked)
            {
                var currentCount = await _cacheService.GetAsync<long?>(LikesKey(request.PostId), cancellationToken);
                if (!currentCount.HasValue)
                {
                    var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
                    currentCount = post?.LikesCount ?? 0;
                }
                return new PostLikeResult(true, (int)currentCount.Value, request.PostId, request.UserId);
            }

            try 
            {
                var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
                if (post == null) throw new NotFoundException($"Post with ID {request.PostId} not found.");

                var postLike = new PostLike
                {
                    PostId = request.PostId,
                    UserId = request.UserId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _postLikeRepository.AddAsync(postLike, cancellationToken);
                
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _postRepository.IncrementLikesCountAsync(request.PostId, cancellationToken);

                var redisKey = LikesKey(request.PostId);
                var newCount = await _cacheService.IncrementAsync(redisKey, cancellationToken);

                var result = new PostLikeResult(true, (int)newCount, request.PostId, request.UserId);
                
                var notificationLockKey = $"notif:like:{request.PostId}:{request.UserId}";
                var alreadyNotified = await _cacheService.GetAsync<bool?>(notificationLockKey, cancellationToken);

                if (alreadyNotified == null)
                {
                    await _publishEndpoint.Publish(new PostLikedEvent
                    {
                        PostId = request.PostId,
                        PostOwnerId = post.UserId,
                        ActorId = request.UserId,
                        ActorName = request.UserName,
                        CreatedAt = DateTime.UtcNow
                    }, cancellationToken);

                    // Mark as notified for 2 hours (Sweet spot for intentional re-likes)
                    await _cacheService.SetAsync(notificationLockKey, true, TimeSpan.FromHours(2), null, cancellationToken);
                }

                return result;
            }
            catch (Exception)
            {
                var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
                return new PostLikeResult(true, post?.LikesCount ?? 0, request.PostId, request.UserId);
            }
        }
    }
}