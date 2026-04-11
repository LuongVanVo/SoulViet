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
}