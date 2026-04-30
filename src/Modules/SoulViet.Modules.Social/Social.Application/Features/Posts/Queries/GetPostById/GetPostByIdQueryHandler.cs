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

        var userIdsToFetch = new List<Guid> { post.UserId };
        if (post.OriginalPost != null)
        {
            userIdsToFetch.Add(post.OriginalPost.UserId);
        }

        var userInfos = await _userService.GetUsersMinimalInfoAsync(userIdsToFetch, cancellationToken);
        
        if (userInfos.TryGetValue(post.UserId, out var userInfo))
        {
            response.AuthorName = userInfo.FullName;
            response.AvatarUrl = userInfo.AvatarUrl;
        }

        var locationIdsToFetch = new List<Guid>();
        if (post.CheckinLocationId.HasValue) locationIdsToFetch.Add(post.CheckinLocationId.Value);
        if (post.OriginalPost?.CheckinLocationId != null) locationIdsToFetch.Add(post.OriginalPost.CheckinLocationId.Value);

        Dictionary<Guid, string> locationNames = new();
        if (locationIdsToFetch.Any())
        {
            locationNames = await _soulMapService.GetLocationNamesAsync(locationIdsToFetch, cancellationToken);
        }

        if (post.CheckinLocationId.HasValue && locationNames.TryGetValue(post.CheckinLocationId.Value, out var locName))
        {
            response.CheckinLocationName = locName;
        }

        if (post.OriginalPost != null && response.OriginalPost != null)
        {
            if (userInfos.TryGetValue(post.OriginalPost.UserId, out var opUserInfo))
            {
                response.OriginalPost.AuthorName = opUserInfo.FullName;
                response.OriginalPost.AvatarUrl = opUserInfo.AvatarUrl;
            }

            if (post.OriginalPost.CheckinLocationId.HasValue && locationNames.TryGetValue(post.OriginalPost.CheckinLocationId.Value, out var opLocName))
            {
                response.OriginalPost.CheckinLocationName = opLocName;
            }

            if (request.UserId != Guid.Empty)
            {
                var opLike = await _postLikeRepository.GetPostLikeAsync(post.OriginalPost.Id, request.UserId, cancellationToken);
                response.OriginalPost.IsLiked = opLike != null;
            }
        }

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

