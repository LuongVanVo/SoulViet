using Microsoft.EntityFrameworkCore;
using SoulViet.Modules.Social.Social.Application.Interfaces;
using SoulViet.Modules.Social.Social.Domain.Entities;

namespace SoulViet.Modules.Social.Social.Infrastructure.Persistence.Repositories;

public class PostRepository : IPostRepository
{
    private readonly SocialDbContext _dbContext;

    public PostRepository(SocialDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Post?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Posts.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
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
        _dbContext.Entry(post).Property("IsDeleted").CurrentValue = true;
        return Task.CompletedTask;
    }
}

