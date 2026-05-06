using FluentAssertions;
using Moq;
using PhotoGallery.Core.Application.Interfaces;
using PhotoGallery.Core.Application.Services;
using PhotoGallery.Core.Domain.Entities;
using PhotoGallery.Core.Infrastructure;
using PhotoGallery.Tests.Helpers;

namespace PhotoGallery.Tests.Services
{

    public class AlbumServiceTests
    {
        private readonly AppDbContext _context;
        private readonly AlbumService _service;
        private readonly Mock<IFileService> _fileServiceMock;

        public AlbumServiceTests()
        {
            _context = TestDbContextFactory.Create();
            _fileServiceMock = new Mock<IFileService>();
            _service = new AlbumService(_context, _fileServiceMock.Object);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenAlbumDoesNotExist()
        {
            var result = await _service.DeleteAsync(Guid.NewGuid(), "user-id", false);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteAlbum_WhenUserIsOwner()
        {
            var user = new User
            {
                Id = "owner-id",
                UserName = "owner"
            };
            var album = new Album
            {
                Id = Guid.NewGuid(),
                Title = "Test Album",
                UserId = user.Id,
                User = user,
            };
            _context.Users.Add(user);
            var image = new Image
            {
                Id = Guid.NewGuid(),
                ImagePath = "/uploads/image.jpg",
                ThumbnailPath = "/uploads/thumb.jpg"
            };
            album.Images = [image];
            _context.Users.Add(user);
            _context.Albums.Add(album);
            await _context.SaveChangesAsync();

            var result = await _service.DeleteAsync(album.Id, "owner-id", false);

            result.Should().BeTrue();
            var deletedAlbum = await _context.Albums.FindAsync(album.Id);
            deletedAlbum.Should().BeNull();
            _fileServiceMock.Verify(x => x.DeleteImage(image.ImagePath, image.ThumbnailPath), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenUserIsNotOwner()
        {
            var user = new User
            {
                Id = "owner-id",
                UserName = "owner"
            };
            var album = new Album
            {
                Id = Guid.NewGuid(),
                Title = "Test Album",
                UserId = user.Id,
                User = user,
            };
            _context.Users.Add(user);
            _context.Albums.Add(album);
            await _context.SaveChangesAsync();

            var result = await _service.DeleteAsync(album.Id, "another-user", false);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteAsync_ShouldAllowAdminDelete()
        {
            var user = new User
            {
                Id = "owner-id",
                UserName = "owner"
            };
            var album = new Album
            {
                Id = Guid.NewGuid(),
                Title = "Test Album",
                UserId = user.Id,
                User = user,
            };
            _context.Users.Add(user);
            _context.Albums.Add(album);
            await _context.SaveChangesAsync();

            var result = await _service.DeleteAsync(album.Id, "admin-id", true);

            result.Should().BeTrue();
        }
    }
}