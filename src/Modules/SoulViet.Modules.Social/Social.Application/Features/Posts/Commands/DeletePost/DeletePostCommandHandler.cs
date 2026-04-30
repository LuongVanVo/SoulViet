using MediatR;
using SoulViet.Modules.Social.Social.Application.Exceptions;
using SoulViet.Modules.Social.Social.Application.Features.Posts.Results;
using SoulViet.Modules.Social.Social.Application.Interfaces;
using SoulViet.Modules.Social.Social.Application.Interfaces.Services;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;
using SoulViet.Shared.Application.Interfaces;

namespace SoulViet.Modules.Social.Social.Application.Features.Posts.Commands.DeletePost;

public class DeletePostCommandHandler : IRequestHandler<DeletePostCommand, DeletePostResponse>
{
    private readonly IPostRepository _postRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    private static string SharesKey(Guid postId) => $"post:shares:{postId}";

    public DeletePostCommandHandler(
        IPostRepository postRepository, 
        IUnitOfWork unitOfWork,
        ICacheService cacheService)
    {
        _postRepository = postRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<DeletePostResponse> Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdAsync(request.Id, cancellationToken);

        if (post is null)
        {
            throw new NotFoundException($"Post with id '{request.Id}' was not found.");
        }

        if (post.UserId != request.UserId)
        {
            throw new ForbiddenException("You are not allowed to delete this post.");
        }

        await _postRepository.SoftDeleteAsync(post, cancellationToken);

        if (post.OriginalPostId.HasValue)
        {
            await _postRepository.DecrementSharesCountAsync(post.OriginalPostId.Value, cancellationToken);
            
            var redisKey = SharesKey(post.OriginalPostId.Value);
            var existsInRedis = await _cacheService.GetAsync<long?>(redisKey, cancellationToken);
            if (existsInRedis != null)
            {
                await _cacheService.DecrementAsync(redisKey, cancellationToken);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new DeletePostResponse
        {
            Success = true,
            Message = "Post deleted successfully."
        };
    }
}
