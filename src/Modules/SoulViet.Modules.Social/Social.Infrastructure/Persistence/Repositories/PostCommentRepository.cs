using Microsoft.EntityFrameworkCore;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;
using SoulViet.Modules.Social.Social.Domain.Entities;

namespace SoulViet.Modules.Social.Social.Infrastructure.Persistence.Repositories;

public class PostCommentRepository : IPostCommentRepository
{
    private readonly SocialDbContext _dbContext;

    public PostCommentRepository(SocialDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<PostComment>> GetPostCommentsByPostIdAsync(Guid postId, CancellationToken cancellationToken)
    {
        return await _dbContext.PostComments
            .Where(c => c.PostId == postId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<PostComment?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.PostComments.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task AddAsync(PostComment comment, CancellationToken cancellationToken)
    {
        await _dbContext.PostComments.AddAsync(comment, cancellationToken);
    }

    public void Update(PostComment comment)
    {
        _dbContext.PostComments.Update(comment);
    }

    public Task Delete(PostComment comment, CancellationToken cancellationToken)
    {
        comment.IsDeleted = true;
        _dbContext.PostComments.Update(comment);
        return Task.CompletedTask;
    }
    public async Task<(List<PostComment> Items, int TotalCount)> GetPagedAsync(
       Guid? postId,
       Guid? parentCommentId,
       string sortBy,
       Guid? cursorId,
       DateTime? cursorTime,
       int? cursorLikeCount,
       int limit,
       CancellationToken cancellationToken)
    {
        var query = _dbContext.PostComments.Where(c => !c.IsDeleted);

        if (postId.HasValue)
            query = query.Where(c => c.PostId == postId.Value && c.ParentCommentId == null);

        if (parentCommentId.HasValue)
            query = query.Where(c => c.ParentCommentId == parentCommentId.Value);
        var totalCount = await query.CountAsync(cancellationToken);
        if (cursorId.HasValue && cursorTime.HasValue)
        {
            if (sortBy == "oldest")
            {
                query = query.Where(c =>
                    c.CreatedAt > cursorTime.Value ||
                    (c.CreatedAt == cursorTime.Value && c.Id > cursorId.Value));
            }
            else
            {
                query = query.Where(c =>
                    c.CreatedAt < cursorTime.Value ||
                    (c.CreatedAt == cursorTime.Value && c.Id < cursorId.Value));
            }
        }
        query = sortBy switch
        {
            "oldest" => query.OrderBy(c => c.CreatedAt).ThenBy(c => c.Id),
            _ => query.OrderByDescending(c => c.CreatedAt).ThenByDescending(c => c.Id)
        };

        var items = await query.Take(limit + 1).ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}