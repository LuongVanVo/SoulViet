namespace SoulViet.Shared.Domain.Entities;

public class TourGuideDetail
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid LocalPartnerProfileId { get; set; }
    public LocalPartnerProfile LocalPartnerProfile { get; set; } = null!;

    // Thông tin chuyên môn
    public string LicenseNumber { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public decimal PricePerDay { get; set; }
    public decimal PricePerHour { get; set; }
    public double AverageRating { get; set; }

    public List<string> Languages { get; set; } = new();
    public List<string> Specialties { get; set; } = new();
    public List<string> CoverageProvinces { get; set; } = new();
}