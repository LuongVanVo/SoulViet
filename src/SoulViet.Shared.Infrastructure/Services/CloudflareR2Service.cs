using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SoulViet.Shared.Application.Common.ExternalSettings;
using SoulViet.Shared.Application.DTOs.Media;
using SoulViet.Shared.Application.Exceptions;
using SoulViet.Shared.Application.Interfaces;

using SoulViet.Shared.Domain.Enums;

namespace SoulViet.Shared.Infrastructure.Services;

public class CloudflareR2Service : ICloudflareR2Service
{
    private readonly string[] _allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".mp4", ".mov", ".avi", ".mkv" };
    private readonly long _maxFileSize = 100 * 1024 * 1024; // 100 MB for general/video
    private readonly IAmazonS3 _s3Client;
    private readonly CloudflareR2Settings _cloudflareR2Settings;
    public CloudflareR2Service(IOptions<CloudflareR2Settings> options)
    {
        _cloudflareR2Settings = options.Value;

        var s3Config = new AmazonS3Config
        {
            ServiceURL = _cloudflareR2Settings.ServiceUrl,
            ForcePathStyle = true, // Important for Cloudflare R2
            UseHttp = false, // Use HTTPS
        };

        _s3Client = new AmazonS3Client(_cloudflareR2Settings.AccessKey, _cloudflareR2Settings.SecretKey, s3Config);
    }

    public string GeneratePresignedUrl(string folderName, string fileName, string contentType, int expiresInMinutes = 15)
    {
        var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(fileName)}";
        // Generate key (path file on R2) - Example: place-tourists/fa20ad85..../image.jpg
        var objectKey = string.IsNullOrEmpty(folderName) ? fileName : $"{folderName}/{fileName}";

        var request = new GetPreSignedUrlRequest
        {
            BucketName = _cloudflareR2Settings.BucketName,
            Key = objectKey,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.AddMinutes(expiresInMinutes),
            ContentType = contentType,
        };

        return _s3Client.GetPreSignedURL(request);
    }

    public List<PresignedUrlResponse> GenerateMultiplePresignedUploadUrls(List<FileUploadRequest> files, string folderName)
    {
        var results = new List<PresignedUrlResponse>();

        foreach (var file in files)
        {
            // Tạo unique key cho từng file
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!_allowedExtensions.Contains(extension))
                throw new BadRequestException($"Format file invalid. Only {_allowedExtensions} are allowed.");

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var objectKey = string.IsNullOrEmpty(folderName) ? uniqueFileName : $"{folderName}/{uniqueFileName}";

            var mediaType = extension == ".mp4" || extension == ".mov" || extension == ".avi" || extension == ".mkv"
                ? MediaType.Video
                : MediaType.Image;

            // Sinh Presigned URL (giống logic hàm đơn lẻ)
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _cloudflareR2Settings.BucketName,
                Key = objectKey,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddMinutes(15),
                ContentType = file.ContentType
            };

            request.Metadata.Add("original-name", file.FileName);

            results.Add(new PresignedUrlResponse
            {
                FileName = file.FileName,
                UploadUrl = _s3Client.GetPreSignedURL(request),
                PublicUrl = $"{_cloudflareR2Settings.PublicDomain}/{objectKey}",
                ObjectKey = objectKey,
                MediaType = mediaType
            });
        }

        return results;
    }

    public async Task<MediaUploadResult> UploadImageAsync(IFormFile file, string folderName)
    {
        if (file == null || file.Length == 0)
            throw new BadRequestException("File must not be null or empty.");

        // Create a unique file to avoid name conflicts or overwriting existing files
        var extension = Path.GetExtension(file.FileName);
        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var objectKey = string.IsNullOrEmpty(folderName) ? uniqueFileName : $"{folderName}/{uniqueFileName}";

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);

        var putRequest = new PutObjectRequest
        {
            BucketName = _cloudflareR2Settings.BucketName,
            Key = objectKey,
            InputStream = memoryStream,
            ContentType = file.ContentType,
            DisablePayloadSigning = true // Important for Cloudflare R2
        };

        var response = await _s3Client.PutObjectAsync(putRequest);

        if (response.HttpStatusCode == HttpStatusCode.OK)
        {
            return new MediaUploadResult
            {
                Url = $"{_cloudflareR2Settings.PublicDomain}/{objectKey}",
                ObjectKey = objectKey
            };
        }

        throw new Exception("Failed to upload image to Cloudflare R2.");
    }

    public async Task<List<MediaUploadResult>> UploadMultipleImagesAsync(List<IFormFile> files, string folderName)
    {
        var uploadTasks = files.Select(file => UploadImageAsync(file, folderName));
        var result = await Task.WhenAll(uploadTasks);
        return result.ToList();
    }

    public async Task<bool> DeleteImageAsync(string objectKey)
    {
        try
        {
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _cloudflareR2Settings.BucketName,
                Key = objectKey,
            };

            var response = await _s3Client.DeleteObjectAsync(deleteRequest);
            return response.HttpStatusCode == HttpStatusCode.NoContent ||
                   response.HttpStatusCode == HttpStatusCode.OK;
        }
        catch (Exception)
        {
            return false;
        }
    }
}