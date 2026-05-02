using MassTransit;
using MediatR;
using SoulViet.Shared.Application.Common.Events;
using SoulViet.Shared.Domain.Enums;
using SoulViet.Shared.Application.Exceptions;
using SoulViet.Shared.Application.Interfaces;
using SoulViet.Modules.Social.Social.Domain.Enums;
using SoulViet.Modules.Social.Social.Application.Interfaces;
using SoulViet.Modules.Social.Social.Application.Interfaces.Services;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;
using SoulViet.Modules.Social.Social.Domain.Entities;
using SoulViet.Modules.Social.Social.Application.Features.PostShares.Results;

namespace SoulViet.Modules.Social.Social.Application.Features.PostShares.Commands.Share
{
    public class PostShareCommandHandler : IRequestHandler<PostShareCommand, PostShareResult>
    {
        private readonly IPostRepository _postRepository;
        private readonly IPostShareRepository _postShareRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;
        private readonly IPublishEndpoint _publishEndpoint;

        private static string SharesKey(Guid postId) => $"post:shares:{postId}";

        public PostShareCommandHandler(
            IPostRepository postRepository,
            IPostShareRepository postShareRepository,
            IUnitOfWork unitOfWork,
            ICacheService cacheService,
            IPublishEndpoint publishEndpoint)
        {
            _postRepository = postRepository;
            _postShareRepository = postShareRepository;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<PostShareResult> Handle(PostShareCommand request, CancellationToken cancellationToken)
        {
            var originalPost = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
            if (originalPost == null)
                throw new NotFoundException("Post not found");

            var postShare = new PostShare
            {
                PostId = request.PostId,
                UserId = request.UserId,
                Caption = request.Caption ?? string.Empty,
                ShareType = request.ShareType
            };
            await _postShareRepository.AddAsync(postShare, cancellationToken);

            Post sharedPost = null;
            if (request.ShareType == ShareType.Timeline)
            {
                sharedPost = new Post
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    Content = request.Caption ?? string.Empty,
                    OriginalPostId = originalPost.OriginalPostId ?? originalPost.Id,
                    Status = PostStatus.Published
                };
                await _postRepository.AddAsync(sharedPost, cancellationToken);
            }

            var redisKey = SharesKey(request.PostId);
            var existsInRedis = await _cacheService.GetAsync<long?>(redisKey, cancellationToken);
            if (existsInRedis == null)
                await _cacheService.SetAsync(redisKey, (long)originalPost.SharesCount, cancellationToken: cancellationToken);

            var newCount = await _cacheService.IncrementAsync(redisKey, cancellationToken);

            originalPost.SharesCount = (int)newCount;
            _postRepository.Update(originalPost);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (originalPost.UserId != request.UserId)
            {
                // If it's a timeline share, we want the notification to link to the new post
                // otherwise we link to the original post
                var targetPostId = (request.ShareType == ShareType.Timeline && sharedPost != null) 
                    ? sharedPost.Id 
                    : request.PostId;

                await _publishEndpoint.Publish(new PostSharedEvent
                {
                    PostId = request.PostId,
                    ShareId = targetPostId,
                    PostOwnerId = originalPost.UserId,
                    ActorId = request.UserId,
                    ActorName = request.UserName,
                    Caption = request.Caption ?? string.Empty,
                    ShareType = request.ShareType,
                    CreatedAt = DateTime.UtcNow
                }, cancellationToken);
            }

            string shareUrl = $"/posts/{request.PostId}";
            return new PostShareResult(postShare.Id, (int)newCount, shareUrl);
        }
    }
}
