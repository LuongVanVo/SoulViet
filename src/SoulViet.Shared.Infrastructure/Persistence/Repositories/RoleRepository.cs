using Microsoft.EntityFrameworkCore;
using SoulViet.Shared.Application.Interfaces.Repositories;
using SoulViet.Shared.Domain.Entities;

namespace SoulViet.Shared.Infrastructure.Persistence.Repositories;

public class RoleRepository: IRoleRepository
{
    private readonly SharedDbContext _dbContext;
    public RoleRepository(SharedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Role?> GetRoleByNameAsync(string roleName)
    {
        return await _dbContext.Roles
            .FirstOrDefaultAsync(r => r.Name == roleName);
    }
}