using AutoMapper;
using MediatR;
using SoulViet.Modules.Social.Social.Application.DTOs;
using SoulViet.Modules.Social.Social.Application.Exceptions;
using SoulViet.Modules.Social.Social.Application.Features.PostComments.Results;
using SoulViet.Modules.Social.Social.Application.Interfaces;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;
using SoulViet.Modules.Social.Social.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SoulViet.Modules.Social.Social.Application.Features.PostComments.Commands.CreatePostComment
{
    public class CreatePostCommentCommandHandler : IRequestHandler<CreatePostCommentCommand, PostCommentResponse>
    {
        private readonly IPostRepository _postRepository;
        private readonly IPostCommentRepository _commentRepository;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreatePostCommentCommandHandler(
            IPostRepository postRepository,
            IPostCommentRepository commentRepository,
            IUserService userService,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _postRepository = postRepository;
            _commentRepository = commentRepository;
            _userService = userService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<PostCommentResponse> Handle(CreatePostCommentCommand request, CancellationToken cancellationToken)
        {
            var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
            if (post == null)
            {
                throw new NotFoundException($"Post with ID {request.PostId} not found.");
            }

            if (request.ParentCommentId.HasValue)
            {
                var parentComment = await _commentRepository.GetByIdAsync(request.ParentCommentId.Value, cancellationToken);
                if (parentComment == null)
                {
                    throw new NotFoundException($"Parent comment with ID {request.ParentCommentId.Value} not found.");
                }
            }

            var commentEntity = new PostComment
            {
                Id = Guid.NewGuid(),
                PostId = request.PostId,
                UserId = request.UserId,
                Content = request.Content,
                ParentCommentId = request.ParentCommentId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.UserId.ToString()
            };

            await _commentRepository.AddAsync(commentEntity, cancellationToken);

            post.CommentsCount += 1;
            _postRepository.Update(post);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var userIds = new List<Guid> { request.UserId };
            var usersInfo = await _userService.GetUsersMinimalInfoAsync(userIds, cancellationToken);

            var response = _mapper.Map<PostCommentResponse>(commentEntity);
            if (usersInfo.TryGetValue(commentEntity.UserId, out var user))
            {
                response.FullName = user.FullName;
                response.AvatarUrl = user.AvatarUrl;
            }
            else
            {
                response.FullName = "Anonymous user";
            }

            return response;
        }
    }
}