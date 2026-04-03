using Microsoft.AspNetCore.Mvc;
using SoulViet.Shared.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace SoulViet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MediaController : ControllerBase
{
    private readonly ICloudinaryService _cloudinaryService;
    public MediaController(ICloudinaryService cloudinaryService)
    {
        _cloudinaryService = cloudinaryService;
    }

    [HttpPost("upload")]
    [SwaggerOperation(Summary = "Upload media file",
        Description = "Uploads a media file (image or video) to Cloudinary and returns the URL.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadImage(IFormFile file, [FromQuery] string module = "common")
    {
        try
        {
            var imageUrl = await _cloudinaryService.UploadImageAsync(file, module);

            return Ok(new
            {
                Success = true,
                Message = "Image uploaded successfully",
                Url = imageUrl
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                Success = false,
                Message = "An error occurred while uploading the image",
            });
        }
    }
}