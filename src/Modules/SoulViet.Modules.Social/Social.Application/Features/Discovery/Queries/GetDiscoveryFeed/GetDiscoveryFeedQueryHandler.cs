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
        private readonly ISoulMapService _soulMapService;
        private readonly IMapper _mapper;

        public GetDiscoveryFeedQueryHandler(
            IPostRepository postRepository,
            ISoulMapService soulMapService,
            IMapper mapper)
        {
            _postRepository = postRepository;
            _soulMapService = soulMapService;
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

            // Nếu sort theo nearby hoặc có tọa độ, gọi SoulMapService để lấy danh sách địa điểm
            if (request.Latitude.HasValue && request.Longitude.HasValue)
            {
                nearbyLocationIds = await _soulMapService.GetNearbyLocationIdsAsync(
                    request.Latitude.Value, 
                    request.Longitude.Value, 
                    request.RadiusKm,
                    cancellationToken);
            }

            var (items, totalCount) = await _postRepository.GetDiscoveryPagedAsync(
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

            var edges = postsToReturn.Select(p => new Edge<PostResponse>
            {
                Cursor = CursorHelper.Encode(p.Id, p.CreatedAt, request.SortBy, request.SortBy == "trending" ? p.TrendingScore : null),
                Node = _mapper.Map<PostResponse>(p)
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
