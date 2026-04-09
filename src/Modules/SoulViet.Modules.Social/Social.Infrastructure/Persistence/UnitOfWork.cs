using SoulViet.Modules.Social.Social.Application.Interfaces;

namespace SoulViet.Modules.Social.Social.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly SocialDbContext _dbContext;

    public UnitOfWork(SocialDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}

