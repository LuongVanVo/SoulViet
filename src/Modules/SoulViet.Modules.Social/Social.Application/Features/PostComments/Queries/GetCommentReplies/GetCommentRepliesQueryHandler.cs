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

namespace SoulViet.Modules.Social.Social.Application.Features.PostComments.Queries.GetCommentReplies;

public class GetCommentRepliesQueryHandler : IRequestHandler<GetCommentRepliesQuery, Connection<PostCommentResponse>?>
{
    private readonly IPostCommentRepository _postCommentRepository;
    private readonly SocialDbContext _dbContext;
    private readonly IUserService _userService;

    public GetCommentRepliesQueryHandler(
        IPostCommentRepository postCommentRepository,
        SocialDbContext dbContext,
        IUserService userService)
    {
        _postCommentRepository = postCommentRepository;
        _dbContext = dbContext;
        _userService = userService;
    }

    public async Task<Connection<PostCommentResponse>?> Handle(GetCommentRepliesQuery request, CancellationToken cancellationToken)
    {
        var parentExists = await _dbContext.PostComments
            .AnyAsync(c => c.Id == request.CommentId && !c.IsDeleted, cancellationToken);

        if (!parentExists) return null;

        Guid? cursorId = null;
        DateTime? cursorTime = null;
        double? cursorScore = null;
        var decodedCursor = CursorHelper.Decode(request.After);
        if (decodedCursor.HasValue &&
            string.Equals(decodedCursor.Value.SortBy, request.SortBy, StringComparison.OrdinalIgnoreCase))
        {
            cursorId = decodedCursor.Value.Id;
            cursorTime = decodedCursor.Value.CreatedAt;
            cursorScore = decodedCursor.Value.Score;
        }

        var (items, totalCount) = await _postCommentRepository.GetPagedAsync(
            postId: null,
            parentCommentId: request.CommentId,
            sortBy: request.SortBy.ToLowerInvariant(),
            cursorId: cursorId,
            cursorTime: cursorTime,
            cursorScore: cursorScore,
            limit: request.First,
            cancellationToken: cancellationToken);

        var hasNextPage = items.Count > request.First;
        var repliesToReturn = items.Take(request.First).ToList();

        var userIds = repliesToReturn.Select(c => c.UserId).Distinct().ToList();
        var userInfos = await _userService.GetUsersMinimalInfoAsync(userIds, cancellationToken);

        var replyIds = repliesToReturn.Select(c => c.Id).ToList();
        var nestedReplyCounts = await _dbContext.PostComments
            .Where(c => c.ParentCommentId != null && replyIds.Contains(c.ParentCommentId.Value) && !c.IsDeleted)
            .GroupBy(c => c.ParentCommentId)
            .Select(g => new { ParentId = g.Key!.Value, Count = g.Count() })
            .ToDictionaryAsync(x => x.ParentId, x => x.Count, cancellationToken);

        var edges = repliesToReturn.Select(c => {
            var userInfo = userInfos.GetValueOrDefault(c.UserId);
            return new Edge<PostCommentResponse>
            {
                Cursor = CursorHelper.Encode(c.Id, c.CreatedAt, request.SortBy),
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
                    RepliesCount = nestedReplyCounts.GetValueOrDefault(c.Id, 0),
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
