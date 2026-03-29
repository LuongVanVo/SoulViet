using Microsoft.EntityFrameworkCore;
using SoulViet.Shared.Application.Interfaces.Repositories;
using SoulViet.Shared.Domain.Entities;

namespace SoulViet.Shared.Infrastructure.Persistence.Repositories
{
    public class UserSessionRepository : IUserSessionRepository
    {
        private readonly SharedDbContext _dbContext;
        public UserSessionRepository(SharedDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task AddSessionAsync(UserSession session)
        {
            await _dbContext.UserSessions.AddAsync(session);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<UserSession?> GetSessionByRefreshTokenAsync(string refreshToken)
        {
            return await _dbContext.UserSessions
                .FirstOrDefaultAsync(x => x.RefreshToken == refreshToken);
        }

        public async Task UpdateSessionAsync(UserSession session)
        {
            _dbContext.UserSessions.Update(session);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RevokeAllSessionsForUserAsync(Guid userId)
        {
            var activeSessions = await _dbContext.UserSessions
                .Where(x => x.UserId == userId && !x.IsRevoked)
                .ToListAsync();

            foreach (var session in activeSessions)
            {
                session.IsRevoked = true;
            }

            _dbContext.UserSessions.UpdateRange(activeSessions);
            await _dbContext.SaveChangesAsync();
        }
    }
}