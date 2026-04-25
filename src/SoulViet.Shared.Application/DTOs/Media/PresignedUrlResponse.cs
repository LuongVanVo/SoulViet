namespace SoulViet.Shared.Application.DTOs.Media;

public class PresignedUrlResponse
{
    public string FileName { get; set; } = string.Empty; // Original file name
    public string UploadUrl { get; set; } = string.Empty; // URL to which the client can upload the file
    public string PublicUrl { get; set; } = string.Empty; // URL to access the file after upload
    public string ObjectKey { get; set; } = string.Empty; // The key or path of the file in the storage system, useful for later reference (e.g., deletion)
}