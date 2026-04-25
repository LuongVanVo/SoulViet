using SoulViet.Shared.Domain.Enums;

namespace SoulViet.Modules.Social.Social.Application.DTOs;

public class MediaItemDto
{
    public string Url { get; set; } = string.Empty;
    public string ObjectKey { get; set; } = string.Empty;
    public MediaType MediaType { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public long? FileSizeBytes { get; set; }
    public int SortOrder { get; set; }
}
