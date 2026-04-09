using MediatR;
using SoulViet.Modules.Social.Social.Application.Exceptions;
using SoulViet.Modules.Social.Social.Application.Features.Posts.Results;
using SoulViet.Modules.Social.Social.Application.Interfaces;

namespace SoulViet.Modules.Social.Social.Application.Features.Posts.Commands.DeletePost;

public class DeletePostCommandHandler : IRequestHandler<DeletePostCommand, DeletePostResponse>
{
    private readonly IPostRepository _postRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeletePostCommandHandler(IPostRepository postRepository, IUnitOfWork unitOfWork)
    {
        _postRepository = postRepository;
        _unitOfWork = unitOfWork;
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
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new DeletePostResponse
        {
            Success = true,
            Message = "Post deleted successfully."
        };
    }
}

