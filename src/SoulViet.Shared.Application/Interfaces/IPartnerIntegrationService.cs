namespace SoulViet.Shared.Application.Interfaces;

public interface IPartnerIntegrationService
{
    Task<Dictionary<Guid, string>> GetPartnerNamesAsync(IEnumerable<Guid> partnerIds, CancellationToken cancellationToken = default);
}