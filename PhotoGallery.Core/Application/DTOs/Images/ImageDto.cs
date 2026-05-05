namespace PhotoGallery.Core.Application.DTOs.Images
{
    public class ImageDto
    {
        public Guid Id { get; set; }
        public string ImagePath { get; set; } = string.Empty;
        public string ThumbnailPath { get; set; } = string.Empty;
        public int Likes { get; set; }
        public int Dislikes { get; set; }
    }
}
