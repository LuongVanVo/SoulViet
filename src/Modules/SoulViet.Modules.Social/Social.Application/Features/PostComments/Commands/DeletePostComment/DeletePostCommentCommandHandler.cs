using MediatR;
using SoulViet.Modules.Social.Social.Application.Features.PostComments.Results;
using SoulViet.Modules.Social.Social.Application.Interfaces;
using SoulViet.Modules.Social.Social.Application.Interfaces.Services;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SoulViet.Modules.Social.Social.Application.Features.PostComments.Commands.DeletePostComment
{
    public class DeletePostCommentCommandHandler : IRequestHandler<DeletePostCommentCommand, DeletePostCommentResponse>
    {
        private readonly IPostCommentRepository _postCommentRepository;
        private readonly IPostRepository _postRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommentEventService _commentEventService;

        public DeletePostCommentCommandHandler(
            IPostCommentRepository postCommentRepository, 
            IPostRepository postRepository,
            IUnitOfWork unitOfWork,
            ICommentEventService commentEventService)
        {
            _postCommentRepository = postCommentRepository;
            _postRepository = postRepository;
            _unitOfWork = unitOfWork;
            _commentEventService = commentEventService;
        }

        public async Task<DeletePostCommentResponse> Handle(DeletePostCommentCommand request, CancellationToken cancellationToken)
        {
            var comment = await _postCommentRepository.GetByIdAsync(request.Id, cancellationToken);
            if (comment == null)
            {
                return new DeletePostCommentResponse
                {
                    Success = false,
                    Message = $"Comment with ID {request.Id} not found."
                };
            }
            if (comment.UserId != request.UserId)
            {
                return new DeletePostCommentResponse
                {
                    Success = false,
                    Message = "You are not allowed to delete this comment."
                };
            }

            var postId = comment.PostId;
            await _postCommentRepository.Delete(comment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _postRepository.UpdateCommentsCountAsync(postId, -1, cancellationToken);
            var post = await _postRepository.GetByIdAsync(postId, cancellationToken);

            var eventPayload = CommentStreamEvent.Deleted(request.Id, postId, post?.CommentsCount ?? 0);
            await _commentEventService.PublishCommentAsync(postId, eventPayload, cancellationToken);

            return new DeletePostCommentResponse
            {
                Success = true,
                Message = "Comment deleted successfully."
            };
        }
    }
}
