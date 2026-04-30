using Microsoft.EntityFrameworkCore;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;
using SoulViet.Modules.Social.Social.Domain.Entities;
using SoulViet.Modules.Social.Social.Infrastructure.Persistence;

namespace SoulViet.Modules.Social.Social.Infrastructure.Persistence.Repositories
{
    public class UserFollowerRepository : IUserFollowerRepository
    {
        private readonly SocialDbContext _context;

        public UserFollowerRepository(SocialDbContext context)
        {
            _context = context;
        }

        public async Task<UserFollower?> GetAsync(Guid followerId, Guid followingId, CancellationToken cancellationToken = default)
        {
            return await _context.UserFollowers
                .FirstOrDefaultAsync(x => x.FollowerId == followerId && x.FollowingId == followingId, cancellationToken);
        }

        public async Task<IEnumerable<UserFollower>> GetFollowersAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.UserFollowers
                .Where(x => x.FollowingId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        public async Task<int> GetFollowersCountAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.UserFollowers.CountAsync(x => x.FollowingId == userId, cancellationToken);
        }

        public async Task<IEnumerable<UserFollower>> GetFollowingAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.UserFollowers
                .Where(x => x.FollowerId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        public async Task<int> GetFollowingCountAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.UserFollowers.CountAsync(x => x.FollowerId == userId, cancellationToken);
        }
        public async Task<(List<UserFollower> Items, int TotalCount)> GetPagedFollowersAsync(
            Guid userId,
            string sortBy,
            Guid? cursorId,
            DateTime? cursorTime,
            int limit,
            CancellationToken cancellationToken)
        {
            var query = _context.UserFollowers.Where(f => f.FollowingId == userId);
            var totalCount = await query.CountAsync(cancellationToken);
            if (sortBy == "newest")
            {
                if (cursorTime.HasValue && cursorId.HasValue)
                {
                    query = query.Where(x => x.CreatedAt < cursorTime.Value ||
                                            (x.CreatedAt == cursorTime.Value && x.FollowerId.CompareTo(cursorId.Value) < 0));
                }
                query = query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.FollowerId);
            }
            var items = await query.Take(limit + 1).ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<(List<UserFollower> Items, int TotalCount)> GetPagedFollowingAsync(
            Guid userId,
            string sortBy,
            Guid? cursorId,
            DateTime? cursorTime,
            int limit,
            CancellationToken cancellationToken)
        {
            var query = _context.UserFollowers.Where(f => f.FollowerId == userId);

            var totalCount = await query.CountAsync(cancellationToken);

            if (sortBy == "newest")
            {
                if (cursorTime.HasValue && cursorId.HasValue)
                {
                    query = query.Where(x => x.CreatedAt < cursorTime.Value ||
                                            (x.CreatedAt == cursorTime.Value && x.FollowingId.CompareTo(cursorId.Value) < 0));
                }
                query = query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.FollowingId);
            }

            var items = await query.Take(limit + 1).ToListAsync(cancellationToken);

            return (items, totalCount);
        }
        public async Task AddAsync(UserFollower userFollower, CancellationToken cancellationToken = default)
        {
            await _context.UserFollowers.AddAsync(userFollower, cancellationToken);
        }

        public void Remove(UserFollower userFollower)
        {
            _context.UserFollowers.Remove(userFollower);
        }

        public async Task<bool> ExistsAsync(Guid followerId, Guid followingId, CancellationToken cancellationToken = default)
        {
            return await _context.UserFollowers
                .AnyAsync(x => x.FollowerId == followerId && x.FollowingId == followingId, cancellationToken);
        }
    }
}
