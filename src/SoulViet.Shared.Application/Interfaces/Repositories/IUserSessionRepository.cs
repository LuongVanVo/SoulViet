using SoulViet.Shared.Domain.Entities;

namespace SoulViet.Shared.Application.Interfaces.Repositories
{
    public interface IUserSessionRepository
    {
        Task AddSessionAsync(UserSession session);
        Task<UserSession?> GetSessionByRefreshTokenAsync(string refreshToken);
        Task UpdateSessionAsync(UserSession session);

        // Delete/Revoke all session login of a user
        Task RevokeAllSessionsForUserAsync(Guid userId);
    }
}