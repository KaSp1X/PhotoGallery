using Microsoft.AspNetCore.Http;

namespace PhotoGallery.Core.Application.Interfaces
{
    public interface IFileService
    {
        Task<(string imagePath, string thumbnailPath)> SaveImageAsync(IFormFile file);
        void DeleteImage(string imagePath, string thumbnailPath);
    }
}
