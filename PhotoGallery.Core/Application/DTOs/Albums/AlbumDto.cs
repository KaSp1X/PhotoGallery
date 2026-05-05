namespace PhotoGallery.Core.Application.DTOs.Albums
{
    public class AlbumDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string OwnerUserName { get; set; } = string.Empty;
        public string? CoverImagePath { get; set; }
    }
}
