using Microsoft.EntityFrameworkCore;
using SoulViet.Shared.Application.Interfaces.Repositories;
using SoulViet.Shared.Domain.Entities;

namespace SoulViet.Shared.Infrastructure.Persistence.Repositories;

public class UserAddressRepository : IUserAddressRepository
{
    private readonly SharedDbContext _dbContext;
    public UserAddressRepository(SharedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<UserAddress>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserAddresses
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.IsDefault)
            .ThenByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<UserAddress?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserAddresses
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<int> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserAddresses
            .CountAsync(x => x.UserId == userId, cancellationToken);
    }

    public async Task<UserAddress?> GetDefaultAddressByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserAddresses
            .FirstOrDefaultAsync(x => x.UserId == userId && x.IsDefault, cancellationToken);
    }

    public async Task AddAsync(UserAddress address, CancellationToken cancellationToken = default)
    {
        await _dbContext.UserAddresses.AddAsync(address, cancellationToken);
    }

    public void Update(UserAddress address)
    {
        _dbContext.UserAddresses.Update(address);
    }

    public void Remove(UserAddress address)
    {
        _dbContext.UserAddresses.Remove(address);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}