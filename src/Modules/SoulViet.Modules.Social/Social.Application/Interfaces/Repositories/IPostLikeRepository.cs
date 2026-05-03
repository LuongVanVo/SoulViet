using SoulViet.Modules.Social.Social.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoulViet.Modules.Social.Social.Application.Interfaces.Repositories
{
    public interface IPostLikeRepository
    {
        Task<PostLike?> GetPostLikeAsync(Guid postId, Guid userId, CancellationToken cancellationToken = default);
        Task<List<Guid>> GetLikedPostIdsAsync(Guid userId, IEnumerable<Guid> postIds, CancellationToken cancellationToken = default);
        Task<List<PostLike>> GetLikersPagedAsync(Guid postId, int limit, DateTime? cursorCreatedAt, Guid? cursorUserId, CancellationToken cancellationToken = default);
        Task AddAsync(PostLike postLike, CancellationToken cancellationToken = default);
        void Remove(PostLike postLike);
    }
}
