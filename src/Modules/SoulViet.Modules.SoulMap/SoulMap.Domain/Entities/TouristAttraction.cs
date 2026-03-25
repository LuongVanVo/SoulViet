using NetTopologySuite.Geometries;
using SoulViet.Shared.Domain.Common;
using SoulViet.Shared.Domain.Enums;

namespace SoulViet.Modules.SoulMap.SoulMap.Domain.Entities
{
    public class TouristAttraction : BaseAuditableEntity
    {
        public Guid? PartnerId { get; set; }
        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public string PlaceId { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public Guid ProvinceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string OperationHours { get; set; } = string.Empty;
        public Point Location { get; set; } = null!;
        public double RatingScore { get; set; }
        public int ReviewCount { get; set; }
        public string ReferencePrice { get; set; } = string.Empty;
        public List<string> AllTypes { get; set; } = new();
        public List<string> Activities { get; set; } = new();
        public List<string> TopReviews { get; set; } = new();
        public PlaceMediaInfo Media { get; set; } = new();
        public VibeTag VibeTag { get; set; }
        public string BudgetTag { get; set; } = string.Empty;
        public string AiContext { get; set; } = string.Empty;
        public Province Province { get; set; } = null!;
    }
}