using Microsoft.EntityFrameworkCore;
using SoulViet.Shared.Application.Interfaces;
using SoulViet.Shared.Infrastructure.Persistence;

namespace SoulViet.Shared.Infrastructure.Services;

public class PartnerIntegrationService : IPartnerIntegrationService
{
    private readonly SharedDbContext _sharedDbContext;
    public PartnerIntegrationService(SharedDbContext sharedDbContext)
    {
        _sharedDbContext = sharedDbContext;
    }

    public async Task<Dictionary<Guid, string>> GetPartnerNamesAsync(IEnumerable<Guid> partnerIds, CancellationToken cancellationToken = default)
    {
        return await _sharedDbContext.Users
            .Where(u => partnerIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.FullName, cancellationToken);
    }
}