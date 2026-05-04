namespace PhotoGallery.Core.Domain.Entities
{
    public class Album
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;
        public ICollection<Image> Images { get; set; } = [];
    }
}
