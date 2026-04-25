using SoulViet.Modules.Social.Social.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoulViet.Modules.Social.Social.Application.Interfaces.Repositories
{
    public interface IComboExperienceRepository
    {
        Task<SocialComboExperience?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task AddAsync(SocialComboExperience comboExperience, CancellationToken cancellationToken);
        void Update(SocialComboExperience comboExperience);
        Task SoftDeleteAsync(SocialComboExperience comboExperience, CancellationToken cancellationToken);
        Task<(IEnumerable<SocialComboExperience> Items, int TotalCount)> GetByGuideIdAsync(Guid guideId, int page, int pageSize, CancellationToken cancellationToken);
    }
}
