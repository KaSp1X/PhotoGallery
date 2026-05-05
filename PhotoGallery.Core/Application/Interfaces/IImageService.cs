using PhotoGallery.Core.Application.Common;
using PhotoGallery.Core.Application.DTOs.Images;

namespace PhotoGallery.Core.Application.Interfaces
{
    public interface IImageService
    {
        Task UploadAsync(UploadImageDto dto, string currentUserId, bool isAdmin);
        Task<PagedResult<ImageDto>> GetAlbumImagesAsync(Guid albumId, int page);
        Task<bool> DeleteAsync(Guid imageId, string currentUserId, bool isAdmin);
        Task<bool> LikeAsync(Guid imageId, string currentUserId);
        Task<bool> DislikeAsync(Guid imageId, string currentUserId);
    }
}
