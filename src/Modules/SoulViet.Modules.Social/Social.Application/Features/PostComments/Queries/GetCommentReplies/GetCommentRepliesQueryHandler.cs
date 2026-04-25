using MediatR;
using Microsoft.EntityFrameworkCore;
using SoulViet.Modules.Social.Social.Application.Common.Pagination;
using SoulViet.Modules.Social.Social.Application.Features.PostComments.Results;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;
using SoulViet.Modules.Social.Social.Infrastructure.Persistence;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SoulViet.Modules.Social.Social.Application.Features.PostComments.Queries.GetCommentReplies;

public class GetCommentRepliesQueryHandler : IRequestHandler<GetCommentRepliesQuery, Connection<PostCommentResponse>?>
{
    private readonly IPostCommentRepository _postCommentRepository;
    private readonly SocialDbContext _dbContext;

    public GetCommentRepliesQueryHandler(
        IPostCommentRepository postCommentRepository,
        SocialDbContext dbContext)
    {
        _postCommentRepository = postCommentRepository;
        _dbContext = dbContext;
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

        var edges = repliesToReturn.Select(c => new Edge<PostCommentResponse>
        {
            Cursor = CursorHelper.Encode(c.Id, c.CreatedAt, request.SortBy),
            Node = new PostCommentResponse
            {
                Id = c.Id,
                PostId = c.PostId,
                UserId = c.UserId,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                ParentCommentId = c.ParentCommentId,
                Success = true
            }
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
