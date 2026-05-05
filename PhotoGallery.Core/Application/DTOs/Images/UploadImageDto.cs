using Microsoft.AspNetCore.Http;

namespace PhotoGallery.Core.Application.DTOs.Images
{
    public class UploadImageDto
    {
        public Guid AlbumId { get; set; }
        public IFormFile File { get; set; } = null!;
    }
}