using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhotoGallery.Core.Application.DTOs.Albums;
using PhotoGallery.Core.Application.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PhotoGallery.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlbumsController(IAlbumService albumService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1)
        {
            var result = await albumService.GetPagedAsync(page);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyAlbums(int page = 1)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await albumService.GetUserAlbumsAsync(userId!, page);

            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(CreateAlbumDto dto)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            await albumService.CreateAsync(dto, userId!);

            return Ok();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            var deleted = await albumService.DeleteAsync(id, userId!, isAdmin);

            if (!deleted)
                return Forbid();

            return NoContent();
        }
    }
}
