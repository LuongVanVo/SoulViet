namespace SoulViet.Modules.SoulMap.SoulMap.Application.DTOs;

public class AccommodationDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string ProvinceName { get; set; } = string.Empty;

    // Phân loại (Ví dụ: KhachSan, Resort, Homestay...)
    public string Type { get; set; } = string.Empty;

    // Tọa độ
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    // Giá và Phân khúc
    public decimal PriceValue { get; set; }
    public string PriceSegment { get; set; } = string.Empty;

    // Đánh giá & Sao
    public double RatingScore { get; set; }
    public int ReviewCount { get; set; }
    public int? StarRating { get; set; } // Số sao của khách sạn (Ví dụ: 4 sao, 5 sao)
    public string ReviewText { get; set; } = string.Empty;

    // Chi tiết tiện ích
    public string Highlights { get; set; } = string.Empty;
    public string FacilitiesJson { get; set; } = "[]";
    public string BookingUrl { get; set; } = string.Empty;

    // Hình ảnh & Media
    public string MainImage { get; set; } = string.Empty;
    public List<string> LandImages { get; set; } = new();
    public string? VideoUrl { get; set; }

    // Tags
    public string VibeTag { get; set; } = string.Empty;
}