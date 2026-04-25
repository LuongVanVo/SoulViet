using MediatR;
using SoulViet.Modules.Social.Social.Application.Features.PostComments.Results;
using SoulViet.Modules.Social.Social.Application.Interfaces;
using SoulViet.Modules.Social.Social.Application.Interfaces.Services;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoulViet.Modules.Social.Social.Application.Features.PostComments.Commands.DeletePostComment
{
    public class DeletePostCommentCommandHandler : IRequestHandler<DeletePostCommentCommand, DeletePostCommentResponse>
    {
        private readonly IPostCommentRepository _postCommentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeletePostCommentCommandHandler(IPostCommentRepository postCommentRepository, IUnitOfWork unitOfWork)
        {
            _postCommentRepository = postCommentRepository;
            _unitOfWork = unitOfWork;
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
            await _postCommentRepository.Delete(comment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new DeletePostCommentResponse
            {
                Success = true,
                Message = "Comment deleted successfully."
            };
        }
    }
}
