using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PhotoGallery.Core.Domain.Entities;

namespace PhotoGallery.Core.Infrastructure
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<User>(options)
    {
        public DbSet<Album> Albums => Set<Album>();
        public DbSet<Image> Images => Set<Image>();
        public DbSet<ImageReaction> ImageReactions => Set<ImageReaction>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ImageReaction>()
                .HasIndex(x => new { x.UserId, x.ImageId })
                .IsUnique();
        }
    }
}
