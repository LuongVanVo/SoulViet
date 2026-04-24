namespace SoulViet.Modules.SoulMap.SoulMap.Application.DTOs;

public class MediaMigrationDto
{
    public string Id { get; set; } = string.Empty;
    public string PlaceId { get; set; } = string.Empty;
    public string MainImage { get; set; } = string.Empty;
    public List<string> LandImages { get; set; } = new();
}

public class AttractionMediaInfo
{
    public string VideoUrl { get; set; } = string.Empty;
    public string MainImage { get; set; } = string.Empty;
    public List<string> LandImages { get; set; } = new();
}
