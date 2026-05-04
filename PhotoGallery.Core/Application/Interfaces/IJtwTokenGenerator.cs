using PhotoGallery.Core.Domain.Entities;

namespace PhotoGallery.Core.Application.Interfaces
{
    public interface IJtwTokenGenerator
    {
        public Task<string> GenerateToken(User user);
    }
}
