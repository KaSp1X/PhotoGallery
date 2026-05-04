using Microsoft.AspNetCore.Identity;

namespace PhotoGallery.Core.Domain.Entities
{
    public class User : IdentityUser
    {
        public ICollection<Album> Albums { get; set; } = [];
    }
}
