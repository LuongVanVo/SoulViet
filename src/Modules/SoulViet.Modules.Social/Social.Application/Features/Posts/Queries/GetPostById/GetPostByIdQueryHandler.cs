using AutoMapper;
using MediatR;
using SoulViet.Modules.Social.Social.Application.Exceptions;
using SoulViet.Modules.Social.Social.Application.Features.Posts.Results;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;
using SoulViet.Modules.Social.Social.Application.Interfaces.Services;

namespace SoulViet.Modules.Social.Social.Application.Features.Posts.Queries.GetPostById;

public class GetPostByIdQueryHandler : IRequestHandler<GetPostByIdQuery, PostResponse>
{
    private readonly IPostRepository _postRepository;
    private readonly IPostLikeRepository _postLikeRepository;
    private readonly ISoulMapService _soulMapService;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public GetPostByIdQueryHandler(
        IPostRepository postRepository, 
        IPostLikeRepository postLikeRepository, 
        ISoulMapService soulMapService,
        IUserService userService, 
        IMapper mapper)
    {
        _postRepository = postRepository;
        _postLikeRepository = postLikeRepository;
        _soulMapService = soulMapService;
        _userService = userService;
        _mapper = mapper;
    }

    public async Task<PostResponse> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdAsync(request.Id, cancellationToken);

        if (post is null)
        {
            throw new NotFoundException($"Post with id '{request.Id}' was not found.");
        }

        var response = _mapper.Map<PostResponse>(post);

        // Lấy thông tin user
        var userInfos = await _userService.GetUsersMinimalInfoAsync(new[] { post.UserId }, cancellationToken);
        if (userInfos.TryGetValue(post.UserId, out var userInfo))
        {
            response.AuthorName = userInfo.FullName;
            response.AvatarUrl = userInfo.AvatarUrl;
        }

        // Lấy tên địa điểm từ SoulMap
        if (post.CheckinLocationId.HasValue)
        {
            var locationNames = await _soulMapService.GetLocationNamesAsync(new[] { post.CheckinLocationId.Value }, cancellationToken);
            if (locationNames.TryGetValue(post.CheckinLocationId.Value, out var locName))
            {
                response.CheckinLocationName = locName;
            }
        }

        // Kiểm tra like
        if (request.UserId != Guid.Empty)
        {
            var like = await _postLikeRepository.GetPostLikeAsync(post.Id, request.UserId, cancellationToken);
            response.IsLiked = like != null;
        }

        response.Success = true;
        response.Message = "Post retrieved successfully.";
        return response;
    }
}

