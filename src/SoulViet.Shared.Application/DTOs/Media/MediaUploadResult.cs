namespace SoulViet.Shared.Application.DTOs.Media;

public class MediaUploadResult
{
    public string Url { get; set; } = string.Empty; // Link public to show image
    public string ObjectKey { get; set; } = string.Empty; // Path in storage, (to delete or update media)
}