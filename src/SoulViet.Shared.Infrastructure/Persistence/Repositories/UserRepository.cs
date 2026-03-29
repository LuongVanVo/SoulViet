using Microsoft.EntityFrameworkCore;
using SoulViet.Shared.Application.Interfaces.Repositories;
using SoulViet.Shared.Domain.Entities;

namespace SoulViet.Shared.Infrastructure.Persistence.Repositories;

public class UserRepository(SharedDbContext dbContext) : IUserRepository
{
    private readonly SharedDbContext _dbContext = dbContext;

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Id == userId);
    }

    public async Task CreateUserAsync(User user)
    {
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
    }

    public Task RemoveVerificationTokenAsync(User user)
    {
        user.VerficationToken = null;
        user.VerficationTokenExpiry = null;

        _dbContext.Users.Update(user);
        return _dbContext.SaveChangesAsync();
    }

    public Task<List<string>> GetUserRolesAsync(Guid userId)
    {
        return _dbContext.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.Role.Name)
            .ToListAsync();
    }

    public async Task<bool> IsEmailConfirmedAsync(Guid userId)
    {
        return await _dbContext.Users
            .Where(u => u.Id == userId)
            .Select(u => u.IsEmailConfirmed)
            .FirstOrDefaultAsync();
    }

    public async Task UpdateUserAsync(User user)
    {
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();
    }
}