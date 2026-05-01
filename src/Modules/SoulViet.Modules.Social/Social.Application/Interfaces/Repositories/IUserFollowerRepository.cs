using SoulViet.Modules.Social.Social.Domain.Entities;

namespace SoulViet.Modules.Social.Social.Application.Interfaces.Repositories
{
    public interface IUserFollowerRepository
    {
        Task<UserFollower?> GetAsync(Guid followerId, Guid followingId, CancellationToken cancellationToken = default);
        Task<IEnumerable<UserFollower>> GetFollowersAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<int> GetFollowersCountAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<UserFollower>> GetFollowingAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<int> GetFollowingCountAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<(List<UserFollower> Items, int TotalCount)> GetPagedFollowersAsync(
            Guid userId,
            string sortBy,
            Guid? cursorId,
            DateTime? cursorTime,
            int limit,
            CancellationToken cancellationToken);
        Task<(List<UserFollower> Items, int TotalCount)> GetPagedFollowingAsync(
            Guid userId,
            string sortBy,
            Guid? cursorId,
            DateTime? cursorTime,
            int limit,
            CancellationToken cancellationToken);
        Task AddAsync(UserFollower userFollower, CancellationToken cancellationToken = default);
        void Remove(UserFollower userFollower);
        Task<bool> ExistsAsync(Guid followerId, Guid followingId, CancellationToken cancellationToken = default);
        Task<HashSet<Guid>> GetFollowingIdsAsync(Guid followerId, IEnumerable<Guid> followingIds, CancellationToken cancellationToken = default);
        Task<HashSet<Guid>> GetFollowerIdsAsync(Guid followingId, IEnumerable<Guid> followerIds, CancellationToken cancellationToken = default);
    }
}
