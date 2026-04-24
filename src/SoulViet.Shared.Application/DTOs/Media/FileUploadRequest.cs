namespace SoulViet.Shared.Application.DTOs.Media;

public class FileUploadRequest
{
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
}