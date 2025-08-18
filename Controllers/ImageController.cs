using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TsrmWebApi.IServices;

namespace TsrmWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly I_Image _imageUploadService;

        public ImageController(I_Image imageUploadService)
        {
            _imageUploadService = imageUploadService;
        }

     

        [HttpPost("upload/{id}")]
        [Authorize]
        public async Task<IActionResult> UploadImage(int id, IFormFile file)
        {
            try
            {
                var savedPath = await _imageUploadService.SaveImageAsync(file, id);
                return Ok(new { Message = "Image saved successfully", Path = savedPath });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }


        [HttpGet("ImageView/{fileName}")]
        [Authorize]
        public IActionResult GetImage(string fileName)
        {
            try
            {
                return _imageUploadService.GetImageStream(fileName);
            }
            catch (FileNotFoundException)
            {
                return NotFound($"Image '{fileName}' not found on server.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
