namespace PhotoGallery.Core.Domain.Entities
{
    public class Image
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public string ThumbnailPath { get; set; } = string.Empty;
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public Guid AlbumId { get; set; }
        public Album Album { get; set; } = null!;
        public ICollection<ImageReaction> Reactions { get; set; } = [];
    }
}
