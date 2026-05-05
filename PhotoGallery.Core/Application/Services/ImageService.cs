using Microsoft.EntityFrameworkCore;
using PhotoGallery.Core.Application.Common;
using PhotoGallery.Core.Application.DTOs.Images;
using PhotoGallery.Core.Application.Interfaces;
using PhotoGallery.Core.Domain.Entities;
using PhotoGallery.Core.Infrastructure;

namespace PhotoGallery.Core.Application.Services
{
    public class ImageService(AppDbContext context, IFileService fileService) : IImageService
    {
        private const int PageSize = 5;
        public async Task<PagedResult<ImageDto>> GetAlbumImagesAsync(Guid albumId, int page)
        {
            var query = context.Images.Where(i => i.AlbumId == albumId).AsNoTracking();
            var totalCount = await query.CountAsync();
            var images = await query.Skip((page - 1) * PageSize).Take(PageSize).ToListAsync();

            var result = new PagedResult<Image>
            {
                Items = images,
                Page = page,
                PageSize = PageSize,
                TotalCount = totalCount
            };

            return MapPagedResult(result);
        }

        public async Task UploadAsync(UploadImageDto dto, string currentUserId, bool isAdmin)
        {
            var album = await context.Albums.Include(a => a.User).Include(a => a.Images).FirstOrDefaultAsync(a => a.Id == dto.AlbumId);

            if (album == null)
                throw new Exception("Album not found.");

            var isOwner = album.UserId == currentUserId;

            if (!isOwner && !isAdmin)
                throw new UnauthorizedAccessException();

            var (imagePath, thumbnailPath) = await fileService.SaveImageAsync(dto.File);
            var image = new Image
            {
                Id = Guid.NewGuid(),
                AlbumId = dto.AlbumId,
                FileName = dto.File.FileName,
                ImagePath = imagePath,
                ThumbnailPath = thumbnailPath
            };

            await context.Images.AddAsync(image);
            await context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(Guid imageId, string currentUserId, bool isAdmin)
        {
            var image = await context.Images.Include(i => i.Album).FirstOrDefaultAsync(i => i.Id == imageId);

            if (image == null)
                return false;

            var isOwner = image.Album.UserId == currentUserId;

            if (!isOwner && !isAdmin)
                return false;

            fileService.DeleteImage(image.ImagePath, image.ThumbnailPath);
            context.Images.Remove(image);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> LikeAsync(Guid imageId, string currentUserId)
        {
            var image = await context.Images.Include(i => i.Album).FirstOrDefaultAsync(i => i.Id == imageId);

            if (image == null)
                return false;

            var existingReaction = await context.ImageReactions.FirstOrDefaultAsync(r => r.ImageId == imageId && r.UserId == currentUserId);

            if (existingReaction?.IsLike == true)
            {
                context.ImageReactions.Remove(existingReaction);
                image.Likes = Math.Max(0, image.Likes - 1);
            }
            else if (existingReaction?.IsLike == false)
            {
                image.Dislikes = Math.Max(0, image.Dislikes - 1);
                existingReaction.IsLike = true;
                image.Likes++;
            }
            else
            {
                var reaction = new ImageReaction
                {
                    Id = Guid.NewGuid(),
                    ImageId = imageId,
                    UserId = currentUserId,
                    IsLike = true
                };

                await context.ImageReactions.AddAsync(reaction);
                image.Likes++;
            }

            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DislikeAsync(Guid imageId, string currentUserId)
        {
            var image = await context.Images.Include(i => i.Album).FirstOrDefaultAsync(i => i.Id == imageId);

            if (image == null)
                return false;

            var existingReaction = await context.ImageReactions.FirstOrDefaultAsync(r => r.ImageId == imageId && r.UserId == currentUserId);

            if (existingReaction?.IsLike == false)
            {
                context.ImageReactions.Remove(existingReaction);
                image.Dislikes = Math.Max(0, image.Dislikes - 1);
            }
            else if (existingReaction?.IsLike == true)
            {
                image.Likes = Math.Max(0, image.Likes - 1);
                existingReaction.IsLike = false;
                image.Dislikes++;
            }
            else
            {
                var reaction = new ImageReaction
                {
                    Id = Guid.NewGuid(),
                    ImageId = imageId,
                    UserId = currentUserId,
                    IsLike = false
                };

                await context.ImageReactions.AddAsync(reaction);
                image.Dislikes++;
            }

            await context.SaveChangesAsync();

            return true;
        }

        private static ImageDto MapImage(Image image) => new()
        {
            Id = image.Id,
            ImagePath = $"https://localhost:7082{image.ImagePath}",
            ThumbnailPath = $"https://localhost:7082{image.ThumbnailPath}",
            Likes = image.Likes,
            Dislikes = image.Dislikes
        };

        private static PagedResult<ImageDto> MapPagedResult(PagedResult<Image> result) => new()
        {
            Items = result.Items.Select(MapImage),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        };
    }
}
