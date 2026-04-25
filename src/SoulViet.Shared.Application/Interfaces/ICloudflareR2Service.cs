using Microsoft.AspNetCore.Http;
using SoulViet.Shared.Application.DTOs.Media;

namespace SoulViet.Shared.Application.Interfaces;

public interface ICloudflareR2Service
{
    string GeneratePresignedUrl(string folderName, string fileName, string contentType, int expireMinutes = 15);
    List<PresignedUrlResponse> GenerateMultiplePresignedUploadUrls(List<FileUploadRequest> files, string folderName);
    Task<MediaUploadResult> UploadImageAsync(IFormFile file, string folderName);
    Task<List<MediaUploadResult>> UploadMultipleImagesAsync(List<IFormFile> files, string folderName);
    Task<bool> DeleteImageAsync(string objectKey);
}