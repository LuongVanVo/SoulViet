using AutoMapper;
using MediatR;
using SoulViet.Modules.Social.Social.Application.DTOs;
using SoulViet.Modules.Social.Social.Application.Exceptions;
using SoulViet.Modules.Social.Social.Application.Features.PostComments.Results;
using SoulViet.Modules.Social.Social.Application.Interfaces;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoulViet.Modules.Social.Social.Application.Features.PostComments.Commands.UpdatePostComment
{
    public class UpdatePostCommentCommandHandler : IRequestHandler<UpdatePostCommentCommand, PostCommentResponse>
    {
        private readonly IPostCommentRepository _commentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdatePostCommentCommandHandler(IPostCommentRepository commentRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _commentRepository = commentRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<PostCommentResponse> Handle(UpdatePostCommentCommand request, CancellationToken cancellationToken)
        {
            var comment = await _commentRepository.GetByIdAsync(request.Id, cancellationToken);
            if (comment == null)
            {
                throw new NotFoundException($"Comment with ID {request.Id} not found.");
            }
            if (comment.UserId != request.UserId)
            {
                throw new ForbiddenException("You are not allowed to update this comment.");
            }
            comment.Content = request.Content;
            _commentRepository.Update(comment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            var response = _mapper.Map<PostCommentResponse>(comment);
            response.Success = true;
            response.Message = "Comment updated successfully.";
            return response;
        }
    }
}
