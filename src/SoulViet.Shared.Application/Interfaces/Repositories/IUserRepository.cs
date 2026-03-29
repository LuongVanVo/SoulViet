using SoulViet.Shared.Domain.Entities;

namespace SoulViet.Shared.Application.Interfaces.Repositories;

public interface IUserRepository
{
    // Find user by email
    Task<User?> GetUserByEmailAsync(string email);

    // Find user by id
    Task<User?> GetUserByIdAsync(Guid userId);

    // Create new user
    Task CreateUserAsync(User user);

    // Remove verification token when confirm email successfully
     Task RemoveVerificationTokenAsync(User user);

     // get user roles
    Task<List<string>> GetUserRolesAsync(Guid userId);

    // Check user is comfirmed email or not
    Task<bool> IsEmailConfirmedAsync(Guid userId);

    // Update user
    Task UpdateUserAsync(User user);
}