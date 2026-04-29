using AutoMapper;
using MediatR;
using SoulViet.Modules.Social.Social.Application.DTOs;
using SoulViet.Modules.Social.Social.Application.Exceptions;
using SoulViet.Modules.Social.Social.Application.Features.PostComments.Results;
using SoulViet.Modules.Social.Social.Application.Interfaces;
using SoulViet.Modules.Social.Social.Application.Interfaces.Services;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SoulViet.Modules.Social.Social.Application.Features.PostComments.Commands.UpdatePostComment
{
    public class UpdatePostCommentCommandHandler : IRequestHandler<UpdatePostCommentCommand, PostCommentResponse>
    {
        private readonly IPostCommentRepository _commentRepository;
        private readonly IPostRepository _postRepository;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICommentEventService _commentEventService;

        public UpdatePostCommentCommandHandler(
            IPostCommentRepository commentRepository, 
            IPostRepository postRepository,
            IUserService userService,
            IUnitOfWork unitOfWork, 
            IMapper mapper,
            ICommentEventService commentEventService)
        {
            _commentRepository = commentRepository;
            _postRepository = postRepository;
            _userService = userService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _commentEventService = commentEventService;
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

            var post = await _postRepository.GetByIdAsync(comment.PostId, cancellationToken);
            var usersInfo = await _userService.GetUsersMinimalInfoAsync(new List<Guid> { comment.UserId }, cancellationToken);

            var response = _mapper.Map<PostCommentResponse>(comment);
            if (usersInfo.TryGetValue(comment.UserId, out var user))
            {
                response.FullName = user.FullName;
                response.AvatarUrl = user.AvatarUrl;
            }

            response.Success = true;
            response.Message = "Comment updated successfully.";

            // Publish event
            var eventPayload = CommentStreamEvent.Updated(response, post?.CommentsCount ?? 0);
            await _commentEventService.PublishCommentAsync(comment.PostId, eventPayload, cancellationToken);

            return response;
        }
    }
}
