using MediatR;
using Microsoft.EntityFrameworkCore;
using SoulViet.Modules.Social.Social.Application.Common.Pagination;
using SoulViet.Modules.Social.Social.Application.Features.PostComments.Results;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;
using SoulViet.Modules.Social.Social.Application.Interfaces.Services;
using SoulViet.Modules.Social.Social.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SoulViet.Modules.Social.Social.Application.Features.PostComments.Queries.GetPostComments;

public class GetPostCommentsQueryHandler : IRequestHandler<GetPostCommentsQuery, Connection<PostCommentResponse>?>
{
    private readonly IPostCommentRepository _postCommentRepository;
    private readonly SocialDbContext _dbContext;
    private readonly IUserService _userService;

    public GetPostCommentsQueryHandler(
        IPostCommentRepository postCommentRepository,
        SocialDbContext dbContext,
        IUserService userService)
    {
        _postCommentRepository = postCommentRepository;
        _dbContext = dbContext;
        _userService = userService;
    }

    public async Task<Connection<PostCommentResponse>?> Handle(GetPostCommentsQuery request, CancellationToken cancellationToken)
    {
        var postExists = await _dbContext.Posts.AnyAsync(p => p.Id == request.PostId, cancellationToken);
        if (!postExists) return null;

        Guid? cursorId = null;
        DateTime? cursorTime = null;
        double? cursorScore = null;

        var decodedCursor = CursorHelper.Decode(request.After);
        if (decodedCursor.HasValue)
        {
            if (string.Equals(decodedCursor.Value.SortBy, request.SortBy, StringComparison.OrdinalIgnoreCase))
            {
                cursorId = decodedCursor.Value.Id;
                cursorTime = decodedCursor.Value.CreatedAt;
                cursorScore = decodedCursor.Value.Score;
            }
        }

        var (items, totalCount) = await _postCommentRepository.GetPagedAsync(
            request.PostId,
            null, // Only root comments
            request.SortBy.ToLowerInvariant(),
            cursorId,
            cursorTime,
            cursorScore,
            request.First,
            cancellationToken);

        var hasNextPage = items.Count > request.First;
        var commentsToReturn = items.Take(request.First).ToList();

        var userIds = commentsToReturn.Select(c => c.UserId).Distinct().ToList();
        var userInfos = await _userService.GetUsersMinimalInfoAsync(userIds, cancellationToken);

        var rootCommentIds = commentsToReturn.Select(c => c.Id).ToList();
        var replyCounts = await _dbContext.PostComments
            .Where(c => c.ParentCommentId != null && rootCommentIds.Contains(c.ParentCommentId.Value) && !c.IsDeleted)
            .GroupBy(c => c.ParentCommentId)
            .Select(g => new { ParentId = g.Key!.Value, Count = g.Count() })
            .ToDictionaryAsync(x => x.ParentId, x => x.Count, cancellationToken);

        Dictionary<Guid, List<PostCommentResponse>> repliesMap = new();
        if (request.IncludeReplies && rootCommentIds.Any())
        {
            var replies = await _dbContext.PostComments
                .Where(c => c.ParentCommentId != null && rootCommentIds.Contains(c.ParentCommentId.Value) && !c.IsDeleted)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync(cancellationToken);

            var replyUserIds = replies.Select(r => r.UserId).Distinct().ToList();
            var replyUserInfos = await _userService.GetUsersMinimalInfoAsync(replyUserIds, cancellationToken);

            foreach (var reply in replies)
            {
                if (!repliesMap.ContainsKey(reply.ParentCommentId!.Value))
                    repliesMap[reply.ParentCommentId.Value] = new List<PostCommentResponse>();

                var userInfo = replyUserInfos.GetValueOrDefault(reply.UserId);
                repliesMap[reply.ParentCommentId.Value].Add(new PostCommentResponse
                {
                    Id = reply.Id,
                    PostId = reply.PostId,
                    UserId = reply.UserId,
                    FullName = userInfo?.FullName ?? "User",
                    AvatarUrl = userInfo?.AvatarUrl,
                    Content = reply.Content,
                    CreatedAt = reply.CreatedAt,
                    ParentCommentId = reply.ParentCommentId,
                    Success = true
                });
            }
        }

        var edges = commentsToReturn.Select(c => {
            var userInfo = userInfos.GetValueOrDefault(c.UserId);
            return new Edge<PostCommentResponse>
            {
                Cursor = CursorHelper.Encode(c.Id, c.CreatedAt, request.SortBy, null),
                Node = new PostCommentResponse
                {
                    Id = c.Id,
                    PostId = c.PostId,
                    UserId = c.UserId,
                    FullName = userInfo?.FullName ?? "User",
                    AvatarUrl = userInfo?.AvatarUrl,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    ParentCommentId = c.ParentCommentId,
                    RepliesCount = replyCounts.GetValueOrDefault(c.Id, 0),
                    Replies = repliesMap.GetValueOrDefault(c.Id, new List<PostCommentResponse>()),
                    Success = true
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

        return new Connection<PostCommentResponse>
        {
            Edges = edges,
            PageInfo = pageInfo,
            TotalCount = totalCount
        };
    }
}
