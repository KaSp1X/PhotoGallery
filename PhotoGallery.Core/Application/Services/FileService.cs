using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using PhotoGallery.Core.Application.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace PhotoGallery.Core.Application.Services
{
    public class FileService(IWebHostEnvironment environment) : IFileService
    {
        public async Task<(string imagePath, string thumbnailPath)> SaveImageAsync(IFormFile file)
        {
            var uploadsRoot = Path.Combine(environment.WebRootPath, "uploads");
            var fullFolder = Path.Combine(uploadsRoot, "full");
            var thumbnailsFolder = Path.Combine(uploadsRoot, "thumbnails");

            Directory.CreateDirectory(fullFolder);
            Directory.CreateDirectory(thumbnailsFolder);

            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(fullFolder, fileName);
            var thumbnailPath = Path.Combine(thumbnailsFolder, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            using (var image = await Image.LoadAsync(fullPath))
            {
                image.Mutate(x => x.Resize(200, 200));
                await image.SaveAsync(thumbnailPath);
            }

            return ($"/uploads/full/{fileName}", $"/uploads/thumbnails/{fileName}");
        }

        public void DeleteImage(string imagePath, string thumbnailPath)
        {
            var fullPath = Path.Combine(environment.WebRootPath, imagePath.TrimStart('/'));
            var thumbPath = Path.Combine(environment.WebRootPath, thumbnailPath.TrimStart('/'));

            if (File.Exists(fullPath))
                File.Delete(fullPath);

            if (File.Exists(thumbPath))
                File.Delete(thumbPath);
        }
    }
}
