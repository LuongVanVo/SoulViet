using AutoMapper;
using MediatR;
using SoulViet.Modules.Social.Social.Application.DTOs;
using SoulViet.Modules.Social.Social.Application.Exceptions;
using SoulViet.Modules.Social.Social.Application.Features.PostComments.Results;
using SoulViet.Modules.Social.Social.Application.Interfaces;
using SoulViet.Modules.Social.Social.Application.Interfaces.Services;
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
        private readonly ICommentEventService _commentEventService;
        private readonly MassTransit.IPublishEndpoint _publishEndpoint;

        public CreatePostCommentCommandHandler(
            IPostRepository postRepository,
            IPostCommentRepository commentRepository,
            IUserService userService,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICommentEventService commentEventService,
            MassTransit.IPublishEndpoint publishEndpoint)
        {
            _postRepository = postRepository;
            _commentRepository = commentRepository;
            _userService = userService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _commentEventService = commentEventService;
            _publishEndpoint = publishEndpoint;
        }
        public async Task<PostCommentResponse> Handle(CreatePostCommentCommand request, CancellationToken cancellationToken)
        {
            var postExists = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
            if (postExists == null)
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
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _postRepository.UpdateCommentsCountAsync(request.PostId, 1, cancellationToken);
            
            var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);

            var userIds = new List<Guid> { request.UserId };
            var usersInfo = await _userService.GetUsersMinimalInfoAsync(userIds, cancellationToken);

            var response = _mapper.Map<PostCommentResponse>(commentEntity);
            response.Success = true;
            if (usersInfo.TryGetValue(commentEntity.UserId, out var user))
            {
                response.FullName = user.FullName;
                response.AvatarUrl = user.AvatarUrl;
            }
            else
            {
                response.FullName = "User";
            }

            var eventPayload = CommentStreamEvent.Created(response, post?.CommentsCount ?? 0);
            await _commentEventService.PublishCommentAsync(request.PostId, eventPayload, cancellationToken);

            await _publishEndpoint.Publish(new SoulViet.Shared.Application.Common.Events.PostCommentedEvent
            {
                PostId = request.PostId,
                PostOwnerId = post?.UserId ?? Guid.Empty,
                ActorId = request.UserId,
                ActorName = response.FullName,
                CreatedAt = DateTime.UtcNow
            }, cancellationToken);

            return response;
        }
    }
}