using SoulViet.Shared.Domain.Enums;

namespace SoulViet.Shared.Application.DTOs;

public class LocalPartnerInfoDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public PartnerType PartnerType { get; set; }
    public bool IsAuthenticCertified { get; set; }
    public string TaxId { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
}