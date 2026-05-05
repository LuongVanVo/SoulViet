using AutoMapper;
using MassTransit;
using MediatR;
using SoulViet.Modules.Social.Social.Application.Features.Posts.Results;
using SoulViet.Modules.Social.Social.Application.Interfaces;
using SoulViet.Modules.Social.Social.Application.Interfaces.Services;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;
using SoulViet.Modules.Social.Social.Domain.Entities;
using SoulViet.Shared.Domain.Enums;
using SoulViet.Modules.Social.Social.Domain.Enums;
using SoulViet.Shared.Application.Common.Events;

namespace SoulViet.Modules.Social.Social.Application.Features.Posts.Commands.CreatePost;

public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, PostResponse>
{
    private readonly IPostRepository _postRepository;
    private readonly IUserService _userService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreatePostCommandHandler(IPostRepository postRepository, IUserService userService, IUnitOfWork unitOfWork, IMapper mapper, IPublishEndpoint publishEndpoint)
    {
        _postRepository = postRepository;
        _userService = userService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<PostResponse> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var post = new Post
        {
            UserId = request.UserId,
            Content = request.Content,
            Media = request.Media.Select(m => new PostMedia
            {
                Url = m.Url,
                MediaType = m.MediaType,
                ObjectKey = m.ObjectKey,
                Width = m.Width,
                Height = m.Height,
                FileSizeBytes = m.FileSizeBytes,
                SortOrder = m.SortOrder
            }).ToList(),
            TaggedProductIds = request.TaggedProductIds ?? new List<Guid>(),
            VibeTag = request.VibeTag,
            CheckinLocationId = request.CheckinLocationId,
            CheckinLocationName = request.CheckinLocationName,
            AspectRatio = request.AspectRatio,
            LikesCount = 0,
            CommentsCount = 0,
            SharesCount = 0,
            Status = PostStatus.Published
        };

        await _postRepository.AddAsync(post, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = _mapper.Map<PostResponse>(post);

        // Lấy thông tin user
        var userInfos = await _userService.GetUsersMinimalInfoAsync(new[] { post.UserId }, cancellationToken);
        if (userInfos.TryGetValue(post.UserId, out var userInfo))
        {
            response.AuthorName = userInfo.FullName;
            response.AvatarUrl = userInfo.AvatarUrl;
        }

        await _publishEndpoint.Publish(new PostCreatedEvent
        {
            PostId = post.Id,
            UserId = post.UserId,
            HasVibeTag = post.VibeTag != 0,
            HasCheckinLocation = post.CheckinLocationId.HasValue,
            CreatedAt = post.CreatedAt
        }, cancellationToken);

        response.Success = true;
        response.Message = "Post created successfully.";
        return response;
    }
}
