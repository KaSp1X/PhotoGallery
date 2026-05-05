using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhotoGallery.Core.Application.DTOs.Images;
using PhotoGallery.Core.Application.Interfaces;
using PhotoGallery.Core.Application.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PhotoGallery.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagesController(IImageService imageService) : ControllerBase
    {
        [HttpGet("album/{albumId}")]
        public async Task<IActionResult> GetAlbumImages(Guid albumId, int page = 1)
        {
            var result = await imageService.GetAlbumImagesAsync(albumId, page);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] UploadImageDto dto)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            try
            {
                await imageService.UploadAsync(dto, userId!, isAdmin);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return Ok();
        }

        [Authorize]
        [HttpPost("{id}/like")]
        public async Task<IActionResult> Like(Guid id)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            var success = await imageService.LikeAsync(id, userId!);

            if (!success)
                return NotFound();

            return Ok();
        }

        [Authorize]
        [HttpPost("{id}/dislike")]
        public async Task<IActionResult> Dislike(Guid id)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            var success = await imageService.DislikeAsync(id, userId!);

            if (!success)
                return NotFound();

            return Ok();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            var deleted = await imageService.DeleteAsync(id, userId!, isAdmin);

            if (!deleted)
                return Forbid();

            return NoContent();
        }
    }
}
