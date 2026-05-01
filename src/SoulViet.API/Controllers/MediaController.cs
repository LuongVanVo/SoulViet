using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoulViet.Shared.Application.DTOs.Media;
using SoulViet.Shared.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace SoulViet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MediaController : ControllerBase
{
    private readonly ICloudflareR2Service _cloudflareR2Service;
    public MediaController(ICloudflareR2Service cloudflareR2Service)
    {
        _cloudflareR2Service = cloudflareR2Service;
    }

    /// <summary>
    /// [FLOW 1] Upload file directly to Backend, then backend will upload to Cloudflare R2 and return the URL to client
    /// Only apply for small file, thumbnail, avatar, etc. For large file, should use presigned URL to upload directly to Cloudflare R2
    /// </summary>
    [HttpPost("upload")]
    [SwaggerOperation(Summary = "Upload file directly to Backend, then backend will upload to Cloudflare R2 and return the URL to client",
        Description = "Only apply for small file, thumbnail, avatar, etc. For large file, should use presigned URL to upload directly to Cloudflare R2")]
    public async Task<IActionResult> UploadImage(IFormFile file, [FromForm] string folder = "general")
    {
        var result = await _cloudflareR2Service.UploadImageAsync(file, folder);
        return Ok(new
        {
            success = true,
            data = result,
        });
    }

    /// <summary>
    /// [FLOW 2] Request to get Presigned URL to FrontEnd from Backend, then FrontEnd will use this URL to upload file directly to Cloudflare R2, then return the URL to client
    /// Recommended for large file, thumbnail, avatar, etc. For small file, can use flow 1 to upload directly to Backend, then backend will upload to Cloudflare R2 and return the URL to client
    /// </summary>
    [Authorize]
    [HttpGet("presigned-url")]
    [SwaggerOperation(Summary = "Get presigned URL to upload file directly to Cloudflare R2",
        Description =
            "This endpoint will return a presigned URL that client can use to upload file directly to Cloudflare R2. This is recommended for large file, thumbnail, avatar, etc.")]
    public IActionResult GetPresignedUrl([FromQuery] string fileName, [FromQuery] string contentType, [FromQuery] string folderName = "general")
    {
        var presignedUrl = _cloudflareR2Service.GeneratePresignedUrl(folderName, fileName, contentType);

        return Ok(new
        {
            success = true,
            uploadUrl = presignedUrl
        });
    }

    [Authorize]
    [HttpPost("presigned-urls/social-post")]
    [SwaggerOperation(Summary = "Get multiple presigned URLs for social post media upload",
        Description =
            "This endpoint will return multiple presigned URLs that client can use to upload files directly to Cloudflare R2 for social post media. The folder is fixed to 'social-posts' to ensure proper organization of uploaded media.")]
    public IActionResult GetSocialPostUrls([FromBody] List<FileUploadRequest> requests)
    {
        // Ép folder cố định để tránh FE truyền folder sai
        var results = _cloudflareR2Service.GenerateMultiplePresignedUploadUrls(requests, "social-posts");
        return Ok(new { success = true, data = results });
    }

    [Authorize]
    [HttpPost("presigned-urls/attractions")]
    [SwaggerOperation(Summary = "Get multiple presigned URLs for attraction media upload",
        Description =
            "This endpoint will return multiple presigned URLs that client can use to upload files directly to Cloudflare R2 for attraction media. The folder is fixed to 'place-tourists' to ensure proper organization of uploaded media.")]
    public IActionResult GetAttractionUrls([FromBody] List<FileUploadRequest> requests)
    {
        var results = _cloudflareR2Service.GenerateMultiplePresignedUploadUrls(requests, "place-tourists");
        return Ok(new { success = true, data = results });
    }

    [Authorize]
    [HttpDelete("delete")]
    [SwaggerOperation(Summary = "Delete file from Cloudflare R2",
        Description = "This endpoint will delete file from Cloudflare R2 based on the provided object key (file path in R2).")]
    public async Task<IActionResult> DeleteImage([FromQuery] string objectKey)
    {
        var isSuccess = await _cloudflareR2Service.DeleteImageAsync(objectKey);
        if (isSuccess) return Ok(new { success = true, message = "File deleted successfully" });
        else return BadRequest(new { success = false, message = "Failed to delete file" });
    }
}