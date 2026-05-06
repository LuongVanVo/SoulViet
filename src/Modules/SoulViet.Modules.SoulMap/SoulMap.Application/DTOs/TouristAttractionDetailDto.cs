namespace SoulViet.Modules.SoulMap.SoulMap.Application.DTOs;

public class TouristAttractionDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;

    // Thông tin Danh mục & Tỉnh thành
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryIconUrl { get; set; } = string.Empty;
    public string ProvinceName { get; set; } = string.Empty;

    // Mô tả và Vận hành
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string OperationHours { get; set; } = string.Empty;
    public string ReferencePrice { get; set; } = string.Empty;

    // Tọa độ
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    // Đánh giá
    public double RatingScore { get; set; }
    public int ReviewCount { get; set; }

    // Dữ liệu mảng
    public List<string> AllTypes { get; set; } = new();
    public List<string> Activities { get; set; } = new();
    public List<string> TopReviews { get; set; } = new();

    // Hình ảnh & Media
    public string MainImage { get; set; } = string.Empty;
    public List<string> LandImages { get; set; } = new();
    public string? VideoUrl { get; set; }

    // Phân loại Tags
    public string VibeTag { get; set; } = string.Empty;
    public string BudgetTag { get; set; } = string.Empty;
}