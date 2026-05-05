using PhotoGallery.Core.Application.Common;
using PhotoGallery.Core.Application.DTOs.Albums;

namespace PhotoGallery.Core.Application.Interfaces
{
    public interface IAlbumService
    {
        Task<PagedResult<AlbumDto>> GetPagedAsync(int page);
        Task<PagedResult<AlbumDto>> GetUserAlbumsAsync(string userId, int page);
        Task<AlbumDto?> GetByIdAsync(Guid id);
        Task CreateAsync(CreateAlbumDto dto, string userId);
        Task<bool> DeleteAsync(Guid albumId, string currentUserId, bool isAdmin);
    }
}
