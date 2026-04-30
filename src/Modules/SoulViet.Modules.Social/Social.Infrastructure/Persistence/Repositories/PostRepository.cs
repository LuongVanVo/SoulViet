using Microsoft.EntityFrameworkCore;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;
using SoulViet.Modules.Social.Social.Domain.Entities;
using SoulViet.Modules.Social.Social.Domain.Enums;
using SoulViet.Shared.Domain.Enums;

namespace SoulViet.Modules.Social.Social.Infrastructure.Persistence.Repositories;

public class PostRepository : IPostRepository
{
    private readonly SocialDbContext _dbContext;

    public PostRepository(SocialDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Post?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Posts
            .Include(p => p.Media)
            .Include(p => p.OriginalPost)
                .ThenInclude(op => op!.Media)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }
    public async Task<int> GetPostsCountByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _dbContext.Posts
            .CountAsync(p => p.UserId == userId && !p.IsDeleted && p.Status == PostStatus.Published, cancellationToken);
    }
    public async Task<(IEnumerable<Post> Items, int TotalCount)> GetPostsByUserIdAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = _dbContext.Posts
            .Include(p => p.Media)
            .Include(p => p.OriginalPost)
                .ThenInclude(op => op!.Media)
            .Where(p => p.UserId == userId && !p.IsDeleted && p.Status == PostStatus.Published)
            .Where(p => p.OriginalPostId == null || (!p.OriginalPost!.IsDeleted && p.OriginalPost!.Status == PostStatus.Published))
            .OrderByDescending(p => p.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<(List<Post> Items, int TotalCount)> GetPagedByUserIdAsync(
        Guid userId,
        string sortBy,
        Guid? cursorId,
        DateTime? cursorCreatedAt,
        int limit,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Posts
            .Include(p => p.Media)
            .Include(p => p.OriginalPost)
                .ThenInclude(op => op!.Media)
            .Where(p => p.UserId == userId && !p.IsDeleted && p.Status == PostStatus.Published)
            .Where(p => p.OriginalPostId == null || (!p.OriginalPost!.IsDeleted && p.OriginalPost!.Status == PostStatus.Published));

        var totalCount = await query.CountAsync(cancellationToken);

        if (cursorCreatedAt.HasValue && cursorId.HasValue)
        {
            if (sortBy == "newest")
            {
                query = query.Where(p => p.CreatedAt < cursorCreatedAt.Value ||
                                         (p.CreatedAt == cursorCreatedAt.Value && p.Id.CompareTo(cursorId.Value) < 0));
            }
            else if (sortBy == "oldest")
            {
                query = query.Where(p => p.CreatedAt > cursorCreatedAt.Value ||
                                         (p.CreatedAt == cursorCreatedAt.Value && p.Id.CompareTo(cursorId.Value) > 0));
            }
        }

        if (sortBy == "newest")
        {
            query = query.OrderByDescending(p => p.CreatedAt).ThenByDescending(p => p.Id);
        }
        else
        {
            query = query.OrderBy(p => p.CreatedAt).ThenBy(p => p.Id);
        }

        var items = await query.Take(limit + 1).ToListAsync(cancellationToken);

        return (items, totalCount);
    }
    public async Task<(List<Post> Items, int TotalCount)> GetDiscoveryPagedAsync(
        Guid? currentUserId,
        List<Guid>? nearbyLocationIds,
        VibeTag? vibeTag,
        string sortBy,
        Guid? cursorId,
        DateTime? cursorCreatedAt,
        double? cursorScore,
        int limit,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Posts
            .Include(p => p.Media)
            .Include(p => p.OriginalPost)
                .ThenInclude(op => op!.Media)
            .Where(p => !p.IsDeleted && p.Status == PostStatus.Published)
            .Where(p => p.OriginalPostId == null || (!p.OriginalPost!.IsDeleted && p.OriginalPost!.Status == PostStatus.Published));

        if (vibeTag.HasValue)
        {
            query = query.Where(p => p.VibeTag == vibeTag.Value);
        }

        if (nearbyLocationIds != null && nearbyLocationIds.Count > 0)
        {
            query = query.Where(p => p.CheckinLocationId.HasValue && nearbyLocationIds.Contains(p.CheckinLocationId.Value));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        // Sorting & Ranking
        if (sortBy == "trending")
        {
            var now = DateTime.UtcNow;
            
            var rankedQuery = query.Select(p => new
            {
                Post = p,
                Score = (p.LikesCount + p.CommentsCount * 2.0 + p.SharesCount * 3.0 + 10.0 + 
                         (currentUserId.HasValue && p.UserId == currentUserId.Value && (now - p.CreatedAt).TotalMinutes < 1 ? 1000000.0 : 0.0)) / 
                        Math.Pow((now - p.CreatedAt).TotalHours + 2, 1.8)
            });

            if (cursorScore.HasValue && cursorId.HasValue)
            {
                rankedQuery = rankedQuery.Where(x => x.Score < cursorScore.Value || 
                                                    (Math.Abs(x.Score - cursorScore.Value) < 0.0001 && x.Post.Id.CompareTo(cursorId.Value) < 0));
            }

            var itemsWithScore = await rankedQuery
                .OrderByDescending(x => x.Score)
                .ThenByDescending(x => x.Post.Id)
                .Take(limit + 1)
                .ToListAsync(cancellationToken);

            foreach (var item in itemsWithScore)
            {
                item.Post.TrendingScore = item.Score;
            }

            return (itemsWithScore.Select(x => x.Post).ToList(), totalCount);
        }
        else if (sortBy == "nearby")
        {

            if (cursorCreatedAt.HasValue && cursorId.HasValue)
            {
                query = query.Where(p => p.CreatedAt < cursorCreatedAt.Value ||
                                         (p.CreatedAt == cursorCreatedAt.Value && p.Id.CompareTo(cursorId.Value) < 0));
            }

            query = query.OrderByDescending(p => p.CreatedAt).ThenByDescending(p => p.Id);
        }
        else // newest
        {
            if (cursorCreatedAt.HasValue && cursorId.HasValue)
            {
                query = query.Where(p => p.CreatedAt < cursorCreatedAt.Value ||
                                         (p.CreatedAt == cursorCreatedAt.Value && p.Id.CompareTo(cursorId.Value) < 0));
            }
            query = query.OrderByDescending(p => p.CreatedAt).ThenByDescending(p => p.Id);
        }

        var items = await query.Take(limit + 1).ToListAsync(cancellationToken);
        return (items, totalCount);
    }

    public async Task AddAsync(Post post, CancellationToken cancellationToken)
    {
        await _dbContext.Posts.AddAsync(post, cancellationToken);
    }

    public void Update(Post post)
    {
        _dbContext.Posts.Update(post);
    }

    public Task SoftDeleteAsync(Post post, CancellationToken cancellationToken)
    {
        post.IsDeleted = true;
        _dbContext.Posts.Update(post);
        return Task.CompletedTask;
    }

    public void RemoveMedia(IEnumerable<PostMedia> media)
    {
        _dbContext.PostMedia.RemoveRange(media);
    }

    public async Task IncrementLikesCountAsync(Guid postId, CancellationToken cancellationToken)
    {
        await _dbContext.Posts
            .Where(p => p.Id == postId)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.LikesCount, p => p.LikesCount + 1), cancellationToken);
    }

    public async Task DecrementLikesCountAsync(Guid postId, CancellationToken cancellationToken)
    {
        await _dbContext.Posts
            .Where(p => p.Id == postId)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.LikesCount, p => p.LikesCount - 1), cancellationToken);
    }

    public async Task UpdateCommentsCountAsync(Guid postId, int delta, CancellationToken cancellationToken)
    {
        await _dbContext.Posts
            .Where(p => p.Id == postId)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.CommentsCount, p => p.CommentsCount + delta), cancellationToken);
    }

    public async Task DecrementSharesCountAsync(Guid postId, CancellationToken cancellationToken)
    {
        await _dbContext.Posts
            .Where(p => p.Id == postId)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.SharesCount, p => p.SharesCount > 0 ? p.SharesCount - 1 : 0), cancellationToken);
    }
}

