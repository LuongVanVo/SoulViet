using MediatR;
using Microsoft.EntityFrameworkCore;
using SoulViet.Modules.Social.Social.Application.Common.Pagination;
using SoulViet.Modules.Social.Social.Application.DTOs;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;
using SoulViet.Modules.Social.Social.Application.Interfaces.Services;
using SoulViet.Modules.Social.Social.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SoulViet.Modules.Social.Social.Application.Features.PostLikes.Queries.GetPostLikers
{
    public class GetPostLikersQueryHandler : IRequestHandler<GetPostLikersQuery, Connection<PostLikerDto>?>
    {
        private readonly IPostLikeRepository _postLikeRepository;
        private readonly IUserFollowerRepository _userFollowerRepository;
        private readonly SocialDbContext _dbContext;
        private readonly IUserService _userService;

        public GetPostLikersQueryHandler(
            IPostLikeRepository postLikeRepository,
            IUserFollowerRepository userFollowerRepository,
            SocialDbContext dbContext,
            IUserService userService)
        {
            _postLikeRepository = postLikeRepository;
            _userFollowerRepository = userFollowerRepository;
            _dbContext = dbContext;
            _userService = userService;
        }

        public async Task<Connection<PostLikerDto>?> Handle(GetPostLikersQuery request, CancellationToken cancellationToken)
        {
            var postExists = await _dbContext.Posts.AnyAsync(p => p.Id == request.PostId, cancellationToken);
            if (!postExists) return null;

            Guid? cursorId = null;
            DateTime? cursorTime = null;

            var decodedCursor = CursorHelper.Decode(request.After);
            if (decodedCursor.HasValue)
            {
                cursorId = decodedCursor.Value.Id;
                cursorTime = decodedCursor.Value.CreatedAt;
            }

            var items = await _postLikeRepository.GetLikersPagedAsync(
                request.PostId,
                request.First,
                cursorTime,
                cursorId,
                cancellationToken);

            var hasNextPage = items.Count > request.First;
            var likesToReturn = items.Take(request.First).ToList();

            var userIds = likesToReturn.Select(l => l.UserId).ToList();
            var userInfos = await _userService.GetUsersMinimalInfoAsync(userIds, cancellationToken);

            HashSet<Guid> followingIds = new();
            HashSet<Guid> followerIds = new();

            if (request.CurrentUserId.HasValue && userIds.Any())
            {
                followingIds = await _userFollowerRepository.GetFollowingIdsAsync(request.CurrentUserId.Value, userIds, cancellationToken);
                followerIds = await _userFollowerRepository.GetFollowerIdsAsync(request.CurrentUserId.Value, userIds, cancellationToken);
            }

            var edges = likesToReturn.Select(l => {
                var userInfo = userInfos.GetValueOrDefault(l.UserId);
                return new Edge<PostLikerDto>
                {
                    Cursor = CursorHelper.Encode(l.UserId, l.CreatedAt, "newest", null),
                    Node = new PostLikerDto
                    {
                        Id = l.UserId,
                        FullName = userInfo?.FullName ?? "User",
                        AvatarUrl = userInfo?.AvatarUrl,
                        IsLocalPartner = userInfo?.IsLocalPartner ?? false,
                        IsFollowing = followingIds.Contains(l.UserId),
                        IsFollower = followerIds.Contains(l.UserId)
                    }
                };
            }).ToList();

            var pageInfo = new PageInfo
            {
                HasNextPage = hasNextPage,
                HasPreviousPage = false,
                StartCursor = edges.FirstOrDefault()?.Cursor,
                EndCursor = edges.LastOrDefault()?.Cursor
            };

            return new Connection<PostLikerDto>
            {
                Edges = edges,
                PageInfo = pageInfo,
                TotalCount = await _dbContext.PostLikes.CountAsync(pl => pl.PostId == request.PostId, cancellationToken)
            };
        }
    }
}
