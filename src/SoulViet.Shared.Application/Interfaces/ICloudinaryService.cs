using Microsoft.AspNetCore.Http;

namespace SoulViet.Shared.Application.Interfaces;

public interface ICloudinaryService
{
    Task<string> UploadImageAsync(IFormFile file, string folderName);

    Task<bool> DeleteImageAsync(string publicId);
}