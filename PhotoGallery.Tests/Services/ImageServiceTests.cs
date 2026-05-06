using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using PhotoGallery.Core.Application.DTOs.Images;
using PhotoGallery.Core.Application.Interfaces;
using PhotoGallery.Core.Application.Services;
using PhotoGallery.Core.Domain.Entities;
using PhotoGallery.Core.Infrastructure;
using PhotoGallery.Tests.Helpers;

namespace PhotoGallery.Tests.Services
{
    public class ImageServiceTests
    {
        private readonly AppDbContext _context;
        private readonly ImageService _service;
        private readonly Mock<IFileService> _fileServiceMock;

        public ImageServiceTests()
        {
            _context = TestDbContextFactory.Create();
            _fileServiceMock = new Mock<IFileService>();
            _service = new ImageService(_context, _fileServiceMock.Object);
        }

        [Fact]
        public async Task LikeAsync_ShouldAddLike_WhenNoReactionExists()
        {
            var album = new Album
            {
                Id = Guid.NewGuid(),
                UserId = "owner-id"
            };
            var image = new Image
            {
                Id = Guid.NewGuid(),
                Likes = 0,
                Dislikes = 0
            };
            album.Images = [image];
            _context.Images.Add(image);
            _context.Albums.Add(album);
            await _context.SaveChangesAsync();

            var result = await _service.LikeAsync(image.Id, "user-id");

            result.Should().BeTrue();
            image.Likes.Should().Be(1);
            _context.ImageReactions.Count().Should().Be(1);
        }

        [Fact]
        public async Task LikeAsync_ShouldRemoveLike_WhenAlreadyLiked()
        {
            var album = new Album
            {
                Id = Guid.NewGuid(),
                UserId = "owner-id"
            };
            var image = new Image
            {
                Id = Guid.NewGuid(),
                Likes = 1
            };
            var reaction = new ImageReaction
            {
                Id = Guid.NewGuid(),
                ImageId = image.Id,
                UserId = "user-id",
                IsLike = true
            };
            album.Images = [image];
            _context.Images.Add(image);
            _context.Albums.Add(album);
            _context.ImageReactions.Add(reaction);
            await _context.SaveChangesAsync();

            var result = await _service.LikeAsync(image.Id, "user-id");

            result.Should().BeTrue();
            image.Likes.Should().Be(0);
            _context.ImageReactions.Count().Should().Be(0);
        }

        [Fact]
        public async Task LikeAsync_ShouldSwitchDislikeToLike()
        {
            var album = new Album
            {
                Id = Guid.NewGuid(),
                UserId = "owner-id"
            };
            var image = new Image
            {
                Id = Guid.NewGuid(),
                Likes = 0,
                Dislikes = 1
            };
            var reaction = new ImageReaction
            {
                Id = Guid.NewGuid(),
                ImageId = image.Id,
                UserId = "user-id",
                IsLike = false
            };
            album.Images = [image];
            _context.Images.Add(image);
            _context.Albums.Add(album);
            _context.ImageReactions.Add(reaction);
            await _context.SaveChangesAsync();

            await _service.LikeAsync(image.Id, "user-id");

            image.Likes.Should().Be(1);
            image.Dislikes.Should().Be(0);
            reaction.IsLike.Should().BeTrue();
        }

        [Fact]
        public async Task LikeAsync_ShouldNeverMakeLikesNegative()
        {
            var album = new Album
            {
                Id = Guid.NewGuid(),
                UserId = "owner-id"
            };
            var image = new Image
            {
                Id = Guid.NewGuid(),
                Likes = 0
            };
            var reaction = new ImageReaction
            {
                Id = Guid.NewGuid(),
                ImageId = image.Id,
                UserId = "user-id",
                IsLike = true
            };
            album.Images = [image];
            _context.Images.Add(image);
            _context.Albums.Add(album);
            _context.ImageReactions.Add(reaction);
            await _context.SaveChangesAsync();

            await _service.LikeAsync(image.Id, "user-id");

            image.Likes.Should().BeGreaterThanOrEqualTo(0);
        }

        [Fact]
        public async Task DislikeAsync_ShouldAddDislike_WhenNoReactionExists()
        {
            var album = new Album
            {
                Id = Guid.NewGuid(),
                UserId = "owner-id"
            };
            var image = new Image
            {
                Id = Guid.NewGuid(),
                Likes = 0,
                Dislikes = 0
            };
            album.Images = [image];
            _context.Images.Add(image);
            _context.Albums.Add(album);
            await _context.SaveChangesAsync();

            var result = await _service.DislikeAsync(image.Id, "user-id");

            result.Should().BeTrue();
            image.Dislikes.Should().Be(1);
            _context.ImageReactions.Count().Should().Be(1);
        }

        [Fact]
        public async Task DislikeAsync_ShouldRemoveDislike_WhenAlreadyDisliked()
        {
            var album = new Album
            {
                Id = Guid.NewGuid(),
                UserId = "owner-id"
            };
            var image = new Image
            {
                Id = Guid.NewGuid(),
                Dislikes = 1
            };
            var reaction = new ImageReaction
            {
                Id = Guid.NewGuid(),
                ImageId = image.Id,
                UserId = "user-id",
                IsLike = false
            };
            album.Images = [image];
            _context.Images.Add(image);
            _context.Albums.Add(album);
            _context.ImageReactions.Add(reaction);
            await _context.SaveChangesAsync();

            var result = await _service.DislikeAsync(image.Id, "user-id");

            result.Should().BeTrue();
            image.Dislikes.Should().Be(0);
            _context.ImageReactions.Count().Should().Be(0);
        }

        [Fact]
        public async Task DislikeAsync_ShouldSwitchLikeToDislike()
        {
            var album = new Album
            {
                Id = Guid.NewGuid(),
                UserId = "owner-id"
            };
            var image = new Image
            {
                Id = Guid.NewGuid(),
                Likes = 1,
                Dislikes = 0
            };
            var reaction = new ImageReaction
            {
                Id = Guid.NewGuid(),
                ImageId = image.Id,
                UserId = "user-id",
                IsLike = true
            };
            album.Images = [image];
            _context.Images.Add(image);
            _context.Albums.Add(album);
            _context.ImageReactions.Add(reaction);
            await _context.SaveChangesAsync();

            await _service.DislikeAsync(image.Id, "user-id");

            image.Likes.Should().Be(0);
            image.Dislikes.Should().Be(1);
            reaction.IsLike.Should().BeFalse();
        }

        [Fact]
        public async Task DislikeAsync_ShouldNeverMakeDislikesNegative()
        {
            var album = new Album
            {
                Id = Guid.NewGuid(),
                UserId = "owner-id"
            };
            var image = new Image
            {
                Id = Guid.NewGuid(),
                Dislikes = 0
            };
            var reaction = new ImageReaction
            {
                Id = Guid.NewGuid(),
                ImageId = image.Id,
                UserId = "user-id",
                IsLike = false
            };
            album.Images = [image];
            _context.Images.Add(image);
            _context.Albums.Add(album);
            _context.ImageReactions.Add(reaction);
            await _context.SaveChangesAsync();

            await _service.DislikeAsync(image.Id, "user-id");

            image.Dislikes.Should().BeGreaterThanOrEqualTo(0);
        }

        [Fact]
        public async Task UploadAsync_ShouldReturnFalse_WhenAlbumDoesNotExist()
        {
            var fileMock = new Mock<IFormFile>();
            var uploadImage = new UploadImageDto
            {
                AlbumId = Guid.NewGuid(),
                File = fileMock.Object,
            };

            Task act() => _service.UploadAsync(uploadImage, "user-id", false);

            await Assert.ThrowsAsync<Exception>(act);
        }

        [Fact]
        public async Task UploadAsync_ShouldReturnFalse_WhenUserIsNotOwner()
        {
            var user = new User
            {
                Id = "owner-id",
                UserName = "owner"
            };
            var album = new Album
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                User = user
            };
            _context.Users.Add(user);
            _context.Albums.Add(album);
            await _context.SaveChangesAsync();
            var fileMock = new Mock<IFormFile>();
            var uploadImage = new UploadImageDto
            {
                AlbumId = album.Id,
                File = fileMock.Object,
            };

            Task act() => _service.UploadAsync(uploadImage, "another-user", false);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(act);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteImage_WhenUserIsOwner()
        {
            var image = new Image
            {
                Id = Guid.NewGuid(),
                ImagePath = "/uploads/image.jpg",
                ThumbnailPath = "/uploads/thumb.jpg",
                Album = new Album
                {
                    UserId = "owner-id"
                }
            };
            _context.Images.Add(image);
            await _context.SaveChangesAsync();

            var result = await _service.DeleteAsync(image.Id, "owner-id", false);

            result.Should().BeTrue();
            var deletedImage = await _context.Images.FindAsync(image.Id);
            deletedImage.Should().BeNull();
            _fileServiceMock.Verify(x => x.DeleteImage(image.ImagePath, image.ThumbnailPath), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldAllowAdminDelete()
        {
            var image = new Image
            {
                Id = Guid.NewGuid(),
                ImagePath = "/uploads/image.jpg",
                ThumbnailPath = "/uploads/thumb.jpg",
                Album = new Album
                {
                    UserId = "owner-id"
                }
            };
            _context.Images.Add(image);
            await _context.SaveChangesAsync();

            var result = await _service.DeleteAsync(image.Id, "admin-id", true);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenUserIsNotOwner()
        {
            var image = new Image
            {
                Id = Guid.NewGuid(),
                Album = new Album
                {
                    UserId = "owner-id"
                }
            };
            _context.Images.Add(image);
            await _context.SaveChangesAsync();

            var result = await _service.DeleteAsync(image.Id, "another-user", false);

            result.Should().BeFalse();
        }
    }
}