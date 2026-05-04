namespace PhotoGallery.Core.Domain.Entities
{
    public class ImageReaction
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public Guid ImageId { get; set; }
        public bool IsLike { get; set; }
        public Image Image { get; set; } = null!;
    }
}
