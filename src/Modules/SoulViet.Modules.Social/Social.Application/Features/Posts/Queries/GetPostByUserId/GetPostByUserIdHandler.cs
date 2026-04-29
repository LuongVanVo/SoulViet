using AutoMapper;
using MediatR;
using SoulViet.Modules.Social.Social.Application.Common.Pagination;
using SoulViet.Modules.Social.Social.Application.Features.Posts.Results;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;
using SoulViet.Modules.Social.Social.Application.Interfaces.Services;
using System.Linq;

namespace SoulViet.Modules.Social.Social.Application.Features.Posts.Queries.GetPostByUserId
{
    public class GetPostsByUserIdQueryHandler : IRequestHandler<GetPostByUserIdQuery, Connection<PostResponse>?>
    {
        private readonly IPostRepository _postRepository;
        private readonly IPostLikeRepository _postLikeRepository;
        private readonly ISoulMapService _soulMapService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public GetPostsByUserIdQueryHandler(
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

        public async Task<Connection<PostResponse>?> Handle(GetPostByUserIdQuery request, CancellationToken cancellationToken)
        {
            Guid? cursorId = null;
            DateTime? cursorCreatedAt = null;

            var decodedCursor = CursorHelper.Decode(request.After);
            if (decodedCursor.HasValue)
            {
                if (string.Equals(decodedCursor.Value.SortBy, request.SortBy, StringComparison.OrdinalIgnoreCase))
                {
                    cursorId = decodedCursor.Value.Id;
                    cursorCreatedAt = decodedCursor.Value.CreatedAt;
                }
            }

            var (items, totalCount) = await _postRepository.GetPagedByUserIdAsync(
                request.UserId,
                request.SortBy.ToLowerInvariant(),
                cursorId,
                cursorCreatedAt,
                request.First,
                cancellationToken);

            var hasNextPage = items.Count > request.First;
            var postsToReturn = items.Take(request.First).ToList();

            // Lấy thông tin user
            var userIds = postsToReturn.Select(p => p.UserId).Distinct().ToList();
            var userInfos = await _userService.GetUsersMinimalInfoAsync(userIds, cancellationToken);

            // Lấy tên địa điểm từ SoulMap
            var locationIds = postsToReturn.Where(p => p.CheckinLocationId.HasValue).Select(p => p.CheckinLocationId!.Value).Distinct().ToList();
            var locationNames = await _soulMapService.GetLocationNamesAsync(locationIds, cancellationToken);

            // Kiểm tra like
            var likedPostIds = new HashSet<Guid>();
            if (request.CurrentUserId.HasValue)
            {
                var postIds = postsToReturn.Select(p => p.Id).ToList();
                var likedIds = await _postLikeRepository.GetLikedPostIdsAsync(request.CurrentUserId.Value, postIds, cancellationToken);
                likedPostIds = new HashSet<Guid>(likedIds);
            }

            var edges = postsToReturn.Select(p =>
            {
                var response = _mapper.Map<PostResponse>(p);
                if (userInfos.TryGetValue(p.UserId, out var userInfo))
                {
                    response.AuthorName = userInfo.FullName;
                    response.AvatarUrl = userInfo.AvatarUrl;
                }

                // Gán tên địa điểm (Ưu tiên SoulMap > Denormalized Name)
                if (p.CheckinLocationId.HasValue && locationNames.TryGetValue(p.CheckinLocationId.Value, out var locName))
                {
                    response.CheckinLocationName = locName;
                }

                response.IsLiked = likedPostIds.Contains(p.Id);

                return new Edge<PostResponse>
                {
                    Cursor = CursorHelper.Encode(p.Id, p.CreatedAt, request.SortBy, null),
                    Node = response
                };
            }).ToList();

            var pageInfo = new PageInfo
            {
                HasNextPage = hasNextPage,
                HasPreviousPage = false,
                StartCursor = edges.FirstOrDefault()?.Cursor,
                EndCursor = edges.LastOrDefault()?.Cursor
            };

            return new Connection<PostResponse>
            {
                Edges = edges,
                PageInfo = pageInfo,
                TotalCount = totalCount
            };
        }
    }
}
