using SoulViet.Shared.Domain.Common;
using SoulViet.Shared.Domain.Enums;

namespace SoulViet.Shared.Domain.Entities
{
    public class LocalPartnerProfile : BaseAuditableEntity
    {
        public Guid UserId { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public PartnerType PartnerType { get; set; }
        public bool IsAuthenticCertified { get; set; } = false;
        public string TaxId { get; set; } = string.Empty; // Mã số thuế
        public TourGuideDetail? TourGuideDetail { get; set; }
    }
}