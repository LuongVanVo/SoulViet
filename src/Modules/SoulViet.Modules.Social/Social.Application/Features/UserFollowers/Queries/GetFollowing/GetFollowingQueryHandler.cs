using MediatR;
using SoulViet.Modules.Social.Social.Application.DTOs;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;
using SoulViet.Shared.Application.Interfaces;
using SoulViet.Modules.Social.Social.Application.Common.Pagination;
using SoulViet.Modules.Social.Social.Application.Features.UserFollowers.Results;

namespace SoulViet.Modules.Social.Social.Application.Features.UserFollowers.Queries.GetFollowing
{
    public class GetFollowingQueryHandler : IRequestHandler<GetFollowingQuery, Connection<FollowerResponse>?>
    {
        private readonly IUserFollowerRepository _followerRepository;
        private readonly IUserService _userService;

        public GetFollowingQueryHandler(
            IUserFollowerRepository followerRepository,
            IUserService userService)
        {
            _followerRepository = followerRepository;
            _userService = userService;
        }

        public async Task<Connection<FollowerResponse>?> Handle(GetFollowingQuery request, CancellationToken cancellationToken)
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

            var (items, totalCount) = await _followerRepository.GetPagedFollowingAsync(
                request.UserId,
                request.SortBy.ToLowerInvariant(),
                cursorId,
                cursorCreatedAt,
                request.First,
                cancellationToken);

            var hasNextPage = items.Count > request.First;
            var followingToReturn = items.Take(request.First).ToList();

            if (!followingToReturn.Any())
            {
                return new Connection<FollowerResponse>
                {
                    Edges = new List<Edge<FollowerResponse>>(),
                    PageInfo = new PageInfo { HasNextPage = false, HasPreviousPage = false },
                    TotalCount = totalCount
                };
            }

            var followingIds = followingToReturn.Select(x => x.FollowingId).ToList();
            var userInfo = await _userService.GetUsersMinimalInfoAsync(followingIds, cancellationToken);
            
            var edges = new List<Edge<FollowerResponse>>();
            foreach (var f in followingToReturn)
            {
                if (userInfo.TryGetValue(f.FollowingId, out var info))
                {
                    var isFollowing = request.UserId == request.CurrentUserId || 
                                     (request.CurrentUserId != Guid.Empty && await _followerRepository.ExistsAsync(request.CurrentUserId, f.FollowingId, cancellationToken));
                    
                    var response = new FollowerResponse
                    {
                        UserId = f.FollowingId,
                        FullName = info.FullName,
                        AvatarUrl = info.AvatarUrl,
                        IsFollowing = isFollowing
                    };

                    edges.Add(new Edge<FollowerResponse>
                    {
                        Cursor = CursorHelper.Encode(f.FollowingId, f.CreatedAt, request.SortBy, null),
                        Node = response
                    });
                }
            }

            var pageInfo = new PageInfo
            {
                HasNextPage = hasNextPage,
                HasPreviousPage = false,
                StartCursor = edges.FirstOrDefault()?.Cursor,
                EndCursor = edges.LastOrDefault()?.Cursor
            };

            return new Connection<FollowerResponse>
            {
                Edges = edges,
                PageInfo = pageInfo,
                TotalCount = totalCount
            };
        }
    }
}
