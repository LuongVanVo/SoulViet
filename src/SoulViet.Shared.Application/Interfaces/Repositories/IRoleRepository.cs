using SoulViet.Shared.Domain.Entities;

namespace SoulViet.Shared.Application.Interfaces.Repositories;

public interface IRoleRepository
{
    Task<Role?> GetRoleByNameAsync(string roleName);
}