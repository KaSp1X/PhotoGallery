using Microsoft.EntityFrameworkCore;
using PhotoGallery.Core.Application.Common;
using PhotoGallery.Core.Application.DTOs.Albums;
using PhotoGallery.Core.Application.Interfaces;
using PhotoGallery.Core.Domain.Entities;
using PhotoGallery.Core.Infrastructure;

namespace PhotoGallery.Core.Application.Services
{
    public class AlbumService(AppDbContext context, IFileService fileService) : IAlbumService
    {
        private const int PageSize = 5;

        public async Task<PagedResult<AlbumDto>> GetPagedAsync(int page)
        {
            var query = context.Albums.Include(a => a.User).Include(a => a.Images).AsNoTracking();
            var totalCount = await query.CountAsync();
            var albums = await query.OrderByDescending(a => a.CreatedAt).Skip((page - 1) * PageSize).Take(PageSize).ToListAsync();

            var result = new PagedResult<Album>
            {
                Items = albums,
                Page = page,
                PageSize = PageSize,
                TotalCount = totalCount
            };

            return MapPagedResult(result);
        }

        public async Task<PagedResult<AlbumDto>> GetUserAlbumsAsync(string userId, int page)
        {
            var query = context.Albums.Include(a => a.User).Include(a => a.Images).Where(a => a.UserId == userId).AsNoTracking();
            var totalCount = await query.CountAsync();
            var albums = await query.OrderByDescending(a => a.CreatedAt).Skip((page - 1) * PageSize).Take(PageSize).ToListAsync();

            var result = new PagedResult<Album>
            {
                Items = albums,
                Page = page,
                PageSize = PageSize,
                TotalCount = totalCount
            };

            return MapPagedResult(result);
        }

        public async Task<AlbumDto?> GetByIdAsync(Guid id)
        {
            var album = await context.Albums.Include(a => a.User).Include(a => a.Images).FirstOrDefaultAsync(a => a.Id == id);

            if (album == null)
                return null;

            return MapAlbum(album);
        }

        public async Task CreateAsync(CreateAlbumDto dto, string userId)
        {
            var album = new Album
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                UserId = userId
            };

            await context.Albums.AddAsync(album);
            await context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(Guid albumId, string currentUserId, bool isAdmin)
        {
            var album = await context.Albums.Include(a => a.User).Include(a => a.Images).FirstOrDefaultAsync(a => a.Id == albumId);

            if (album == null)
                return false;

            var isOwner = album.UserId == currentUserId;

            if (!isOwner && !isAdmin)
                return false;

            foreach (var image in album.Images)
            {
                fileService.DeleteImage(image.ImagePath, image.ThumbnailPath);
            }

            context.Albums.Remove(album);
            await context.SaveChangesAsync();

            return true;
        }

        private static AlbumDto MapAlbum(Album album) => new()
        {
            Id = album.Id,
            Title = album.Title,
            CreatedAt = album.CreatedAt,
            OwnerUserName = album.User.UserName!,
            CoverImagePath = album.Images.FirstOrDefault()?.ThumbnailPath
        };

        private static PagedResult<AlbumDto> MapPagedResult(PagedResult<Album> result) => new()
        {
            Items = result.Items.Select(MapAlbum),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        };
    }
}
