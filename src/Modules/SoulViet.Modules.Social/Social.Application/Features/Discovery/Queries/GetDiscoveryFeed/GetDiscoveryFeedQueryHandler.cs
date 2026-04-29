using SoulViet.Shared.Domain.Enums;
using AutoMapper;
using MediatR;
using SoulViet.Modules.Social.Social.Application.Common.Pagination;
using SoulViet.Modules.Social.Social.Application.Features.Posts.Results;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;
using SoulViet.Modules.Social.Social.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SoulViet.Modules.Social.Social.Application.Features.Discovery.Queries.GetDiscoveryFeed
{
    public class GetDiscoveryFeedQueryHandler : IRequestHandler<GetDiscoveryFeedQuery, Connection<PostResponse>>
    {
        private readonly IPostRepository _postRepository;
        private readonly IPostLikeRepository _postLikeRepository;
        private readonly ISoulMapService _soulMapService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public GetDiscoveryFeedQueryHandler(
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

        public async Task<Connection<PostResponse>> Handle(GetDiscoveryFeedQuery request, CancellationToken cancellationToken)
        {
            Guid? cursorId = null;
            DateTime? cursorCreatedAt = null;
            double? cursorScore = null;

            var decodedCursor = CursorHelper.Decode(request.After);
            if (decodedCursor.HasValue)
            {
                if (string.Equals(decodedCursor.Value.SortBy, request.SortBy, StringComparison.OrdinalIgnoreCase))
                {
                    cursorId = decodedCursor.Value.Id;
                    cursorCreatedAt = decodedCursor.Value.CreatedAt;
                    cursorScore = decodedCursor.Value.Score;
                }
            }

            List<Guid>? nearbyLocationIds = null;

            if (request.Latitude.HasValue && request.Longitude.HasValue)
            {
                nearbyLocationIds = await _soulMapService.GetNearbyLocationIdsAsync(
                    request.Latitude.Value, 
                    request.Longitude.Value, 
                    request.RadiusKm,
                    cancellationToken);
            }

            var (items, totalCount) = await _postRepository.GetDiscoveryPagedAsync(
                request.CurrentUserId,
                nearbyLocationIds,
                request.VibeTag,
                request.SortBy,
                cursorId,
                cursorCreatedAt,
                cursorScore,
                request.First,
                cancellationToken);

            var hasNextPage = items.Count > request.First;
            var postsToReturn = items.Take(request.First).ToList();
            var userIds = postsToReturn.Select(p => p.UserId).Distinct().ToList();
            var userInfos = await _userService.GetUsersMinimalInfoAsync(userIds, cancellationToken);
            var locationIds = postsToReturn.Where(p => p.CheckinLocationId.HasValue).Select(p => p.CheckinLocationId!.Value).Distinct().ToList();
            var locationNames = await _soulMapService.GetLocationNamesAsync(locationIds, cancellationToken);

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

                if (p.CheckinLocationId.HasValue && locationNames.TryGetValue(p.CheckinLocationId.Value, out var locName))
                {
                    response.CheckinLocationName = locName;
                }

                response.IsLiked = likedPostIds.Contains(p.Id);
                
                return new Edge<PostResponse>
                {
                    Cursor = CursorHelper.Encode(p.Id, p.CreatedAt, request.SortBy, request.SortBy == "trending" ? p.TrendingScore : null),
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
