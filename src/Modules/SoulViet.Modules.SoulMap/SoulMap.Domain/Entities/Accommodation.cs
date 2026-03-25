using NetTopologySuite.Geometries;
using SoulViet.Modules.SoulMap.SoulMap.Domain.Common;
using SoulViet.Modules.SoulMap.SoulMap.Domain.Enums;
using SoulViet.Shared.Domain.Enums;

namespace SoulViet.Modules.SoulMap.SoulMap.Domain.Entities
{
    public class Accommodation : BaseAuditableEntity
    {
        public Guid? PartnerId { get; set; }
        public Guid ProvinceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public AccommodationType Type { get; set; }
        public string Address { get; set; } = string.Empty;

        // Tọa độ địa lý
        public Point Location { get; set; } = null!;
        public decimal PriceValue { get; set; }
        public string PriceSegment { get; set; } = string.Empty;
        public double RatingScore { get; set; }
        public int ReviewCount { get; set; }
        public int? StarRating { get; set; }
        public string ReviewText { get; set; } = string.Empty;
        public string Highlights { get; set; } = string.Empty;
        public string FacilitiesJson { get; set; } = "[]";
        public PlaceMediaInfo Media { get; set; } = new();
        public string BookingUrl { get; set; } = string.Empty;
        public VibeTag VibeTag { get; set; }
        public string AiContext { get; set; } = string.Empty;
        public Province Province { get; set; } = null!;
    }
}