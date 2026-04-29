using Microsoft.EntityFrameworkCore;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;
using SoulViet.Modules.Social.Social.Domain.Entities;

namespace SoulViet.Modules.Social.Social.Infrastructure.Persistence.Repositories
{
    public class PostLikeRepository : IPostLikeRepository
    {
        private readonly SocialDbContext _context;

        public PostLikeRepository(SocialDbContext context)
        {
            _context = context;
        }

        public async Task<PostLike?> GetPostLikeAsync(Guid postId, Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.PostLikes
                .FirstOrDefaultAsync(pl => pl.PostId == postId && pl.UserId == userId, cancellationToken);
        }
        
        public async Task<List<Guid>> GetLikedPostIdsAsync(Guid userId, IEnumerable<Guid> postIds, CancellationToken cancellationToken = default)
        {
            return await _context.PostLikes
                .Where(pl => pl.UserId == userId && postIds.Contains(pl.PostId))
                .Select(pl => pl.PostId)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(PostLike postLike, CancellationToken cancellationToken = default)
        {
            await _context.PostLikes.AddAsync(postLike, cancellationToken);
        }

        public void Remove(PostLike postLike)
        {
            _context.PostLikes.Remove(postLike);
        }
    }
}