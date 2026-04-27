using SoulViet.Modules.Social.Social.Domain.Common;

namespace SoulViet.Modules.Social.Social.Domain.Entities
{
    public class SocialComboExperience : BaseAuditableEntity
    {
        public Guid GuideId { get; set; } 
        public Guid ServicePartnerId { get; set; } 
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? PromotionalPrice { get; set; } 
        public List<string> MediaUrls { get; set; } = new();
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false; 
    }
}
