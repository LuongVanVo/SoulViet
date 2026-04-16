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

namespace SoulViet.Modules.Social.Social.Application.Features.PostComments.Queries.GetPostComments;

public class GetPostCommentsQueryHandler : IRequestHandler<GetPostCommentsQuery, Connection<PostCommentResponse>?>
{
    private readonly IPostCommentRepository _postCommentRepository;
    private readonly SocialDbContext _dbContext;

    public GetPostCommentsQueryHandler(
        IPostCommentRepository postCommentRepository,
        SocialDbContext dbContext)
    {
        _postCommentRepository = postCommentRepository;
        _dbContext = dbContext;
    }

    public async Task<Connection<PostCommentResponse>?> Handle(GetPostCommentsQuery request, CancellationToken cancellationToken)
    {
        var postExists = await _dbContext.Posts.AnyAsync(p => p.Id == request.PostId, cancellationToken);
        if (!postExists) return null;

        Guid? cursorId = null;
        DateTime? cursorTime = null;
        int? cursorLikeCount = null;

        var decodedCursor = CursorHelper.Decode(request.After);
        if (decodedCursor.HasValue)
        {
            if (string.Equals(decodedCursor.Value.SortBy, request.SortBy, StringComparison.OrdinalIgnoreCase))
            {
                cursorId = decodedCursor.Value.Id;
                cursorTime = decodedCursor.Value.CreatedAt;
                cursorLikeCount = decodedCursor.Value.LikeCount;
            }
        }

        var (items, totalCount) = await _postCommentRepository.GetPagedAsync(
            request.PostId,
            null, // Only root comments
            request.SortBy.ToLowerInvariant(),
            cursorId,
            cursorTime,
            cursorLikeCount,
            request.First,
            cancellationToken);

        var hasNextPage = items.Count > request.First;
        var commentsToReturn = items.Take(request.First).ToList();

        var edges = commentsToReturn.Select(c => new Edge<PostCommentResponse>
        {
            Cursor = CursorHelper.Encode(c.Id, c.CreatedAt, request.SortBy, null), 
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
