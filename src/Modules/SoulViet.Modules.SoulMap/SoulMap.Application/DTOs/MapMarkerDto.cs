namespace SoulViet.Modules.SoulMap.SoulMap.Application.DTOs;

public class MapMarkerDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public double Latitude { get; set; }
    public double Longitude { get; set; }

    // Phân biệt mode: Tourist Attraction hay Accommodation
    public string MarkerType { get; set; } = string.Empty;

    // Lọai chi tiết (Ví dụ: Resort, Biển, Bảo tàng, v.v.)
    public string SubType { get; set; } = string.Empty;

    public double RatingScore { get; set; }
    public int ReviewCount { get; set; }
    public string ThumbnailUrl { get; set; } = string.Empty;

    // Thông tin giá cả hoặc phân khúc giá
    public string PriceInfo { get; set; } = string.Empty;
}