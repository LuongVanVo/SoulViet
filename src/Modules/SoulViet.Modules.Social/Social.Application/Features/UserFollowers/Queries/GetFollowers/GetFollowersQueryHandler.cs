using MediatR;
using SoulViet.Modules.Social.Social.Application.DTOs;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;
using SoulViet.Shared.Application.Interfaces;
using SoulViet.Modules.Social.Social.Application.Common.Pagination;
using SoulViet.Modules.Social.Social.Application.Features.UserFollowers.Results;

namespace SoulViet.Modules.Social.Social.Application.Features.UserFollowers.Queries.GetFollowers
{
    public class GetFollowersQueryHandler : IRequestHandler<GetFollowersQuery, Connection<FollowerResponse>?>
    {
        private readonly IUserFollowerRepository _followerRepository;
        private readonly IUserService _userService;

        public GetFollowersQueryHandler(
            IUserFollowerRepository followerRepository,
            IUserService userService)
        {
            _followerRepository = followerRepository;
            _userService = userService;
        }

        public async Task<Connection<FollowerResponse>?> Handle(GetFollowersQuery request, CancellationToken cancellationToken)
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

            var (items, totalCount) = await _followerRepository.GetPagedFollowersAsync(
                request.UserId,
                request.SortBy.ToLowerInvariant(),
                cursorId,
                cursorCreatedAt,
                request.First,
                cancellationToken);

            var hasNextPage = items.Count > request.First;
            var followersToReturn = items.Take(request.First).ToList();

            if (!followersToReturn.Any())
            {
                return new Connection<FollowerResponse>
                {
                    Edges = new List<Edge<FollowerResponse>>(),
                    PageInfo = new PageInfo { HasNextPage = false, HasPreviousPage = false },
                    TotalCount = totalCount
                };
            }

            var followerIds = followersToReturn.Select(x => x.FollowerId).ToList();
            var userInfo = await _userService.GetUsersMinimalInfoAsync(followerIds, cancellationToken);

            var edges = new List<Edge<FollowerResponse>>();
            foreach (var f in followersToReturn)
            {
                if (userInfo.TryGetValue(f.FollowerId, out var info))
                {
                    var isFollowing = request.CurrentUserId != Guid.Empty && 
                                     await _followerRepository.ExistsAsync(request.CurrentUserId, f.FollowerId, cancellationToken);
                    
                    var response = new FollowerResponse
                    {
                        UserId = f.FollowerId,
                        FullName = info.FullName,
                        AvatarUrl = info.AvatarUrl,
                        IsFollowing = isFollowing
                    };

                    edges.Add(new Edge<FollowerResponse>
                    {
                        Cursor = CursorHelper.Encode(f.FollowerId, f.CreatedAt, request.SortBy, null),
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
