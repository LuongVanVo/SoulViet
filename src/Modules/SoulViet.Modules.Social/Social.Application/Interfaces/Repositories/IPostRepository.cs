using SoulViet.Modules.Social.Social.Application.Common.Pagination;
using SoulViet.Modules.Social.Social.Application.DTOs;
using SoulViet.Modules.Social.Social.Domain.Entities;
using SoulViet.Shared.Domain.Enums;

namespace SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;

public interface IPostRepository
{
    Task<Post?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<(IEnumerable<Post> Items, int TotalCount)> GetPostsByUserIdAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken);
    Task<(List<Post> Items, int TotalCount)> GetPagedByUserIdAsync(
        Guid userId,
        string sortBy,
        Guid? cursorId,
        DateTime? cursorCreatedAt,
        int limit,
        CancellationToken cancellationToken);
    Task<(List<Post> Items, int TotalCount)> GetDiscoveryPagedAsync(
        List<Guid>? nearbyLocationIds,
        VibeTag? vibeTag,
        string sortBy,
        Guid? cursorId,
        DateTime? cursorCreatedAt,
        double? cursorScore,
        int limit,
        CancellationToken cancellationToken);
    Task AddAsync(Post post, CancellationToken cancellationToken);
    void Update(Post post);
    Task SoftDeleteAsync(Post post, CancellationToken cancellationToken);
    void RemoveMedia(IEnumerable<PostMedia> media);
}
