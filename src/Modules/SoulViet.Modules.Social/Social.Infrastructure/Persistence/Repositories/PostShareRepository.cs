using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;
using SoulViet.Modules.Social.Social.Domain.Entities;

namespace SoulViet.Modules.Social.Social.Infrastructure.Persistence.Repositories
{
    public class PostShareRepository : IPostShareRepository
    {
        private readonly SocialDbContext _context;

        public PostShareRepository(SocialDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PostShare postShare, CancellationToken cancellationToken = default)
        {
            await _context.PostShares.AddAsync(postShare, cancellationToken);
        }
    }
}
