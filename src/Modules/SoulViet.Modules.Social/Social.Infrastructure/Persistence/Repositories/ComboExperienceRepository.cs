using Microsoft.EntityFrameworkCore;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;
using SoulViet.Modules.Social.Social.Domain.Entities;

namespace SoulViet.Modules.Social.Social.Infrastructure.Persistence.Repositories
{
    public class ComboExperienceRepository : IComboExperienceRepository
    {
        private readonly SocialDbContext _context;

        public ComboExperienceRepository(SocialDbContext context)
        {
            _context = context;
        }
        public async Task<SocialComboExperience?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.SocialComboExperiences
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task AddAsync(SocialComboExperience comboExperience, CancellationToken cancellationToken)
        {
            await _context.SocialComboExperiences.AddAsync(comboExperience, cancellationToken);
        }
        public void Update(SocialComboExperience comboExperience)
        {
            _context.SocialComboExperiences.Update(comboExperience);
        }
        public async Task SoftDeleteAsync(SocialComboExperience comboExperience, CancellationToken cancellationToken)
        {
            comboExperience.IsDeleted = true;
            _context.SocialComboExperiences.Update(comboExperience);
            await Task.CompletedTask;
        }
        public async Task<(IEnumerable<SocialComboExperience> Items, int TotalCount)> GetByGuideIdAsync(
            Guid guideId, int page, int pageSize, CancellationToken cancellationToken)
        {
            var query = _context.SocialComboExperiences
                .Where(c => c.GuideId == guideId && !c.IsDeleted && c.IsActive);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }
    }
}
