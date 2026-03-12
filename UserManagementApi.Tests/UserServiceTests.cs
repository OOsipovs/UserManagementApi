using FluentAssertions;
using Moq;
using UserManagementApi.Application.DTOs;
using UserManagementApi.Application.Interfaces;
using UserManagementApi.Application.Services;
using UserManagementApi.Domain.Entities;
using UserManagementApi.Domain.Interfaces;

namespace UserManagementApi.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _repoMock = new();
        private readonly Mock<IMessagePublisher> _publisherMock = new();
        private readonly UserService _sut;

        public UserServiceTests()
        {
            _sut = new UserService(_repoMock.Object, _publisherMock.Object);
        }

        [Fact]
        public async Task GetUserByIdAsync_WhenUserExists_ReturnsCorrectResponse()
        {
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Username = "testuser",
                Email = "testuser@example.com",
                Profile = new Profile
                {
                    FirstName = "Test",
                    LastName = "User",
                    DateOfBirth = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            };

            _repoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

            var result = await _sut.GetUserByIdAsync(userId);

            result.Should().NotBeNull();
            result!.Id.Should().Be(userId);
            result.Username.Should().Be("testuser");
            result.Email.Should().Be("testuser@example.com");
            result.FirstName.Should().Be("Test");
            result.LastName.Should().Be("User");
        }

        [Fact]
        public async Task GetUserByIdAsync_WhenUserNotFound_ReturnsNull()
        {
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);

            var result = await _sut.GetUserByIdAsync(Guid.NewGuid());

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetUserByIdAsync_WhenUserHasNoProfile_ReturnsFallbackValues()
        {
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Username = "noprofile", Email = "x@example.com", Profile = null };
            _repoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

            var result = await _sut.GetUserByIdAsync(userId);

            result.Should().NotBeNull();
            result!.FirstName.Should().BeEmpty();
            result.LastName.Should().BeEmpty();
            result.DateOfBirth.Should().Be(default);
        }

 
        [Fact]
        public async Task CreateUserAsync_ValidRequest_ReturnsUserResponse()
        {
            var request = new CreateUserRequest
            {
                Username = "testuser",
                Email = "testuser@example.com",
                FirstName = "Test",
                LastName = "User",
                DateOfBirth = new DateTime(1995, 6, 15, 0, 0, 0, DateTimeKind.Utc)
            };

            _repoMock.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _publisherMock.Setup(p => p.PublishUserCreatedAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            var result = await _sut.CreateUserAsync(request);

            result.Should().NotBeNull();
            result.Username.Should().Be("testuser");
            result.Email.Should().Be("testuser@example.com");
            result.FirstName.Should().Be("Test");
            result.LastName.Should().Be("User");
        }

        [Fact]
        public async Task CreateUserAsync_PublishesMessageAfterSave()
        {
            var callOrder = new List<string>();

            _repoMock.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync())
                     .Callback(() => callOrder.Add("save"))
                     .Returns(Task.CompletedTask);
            _publisherMock.Setup(p => p.PublishUserCreatedAsync(It.IsAny<User>()))
                          .Callback(() => callOrder.Add("publish"))
                          .Returns(Task.CompletedTask);

            var request = new CreateUserRequest
            {
                Username = "u",
                Email = "u@x.com",
                FirstName = "F",
                LastName = "L",
                DateOfBirth = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };

            await _sut.CreateUserAsync(request);

            callOrder.Should().ContainInOrder("save", "publish");
        }

        [Fact]
        public async Task CreateUserAsync_PublishesMessageExactlyOnce()
        {
            _repoMock.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _publisherMock.Setup(p => p.PublishUserCreatedAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            var request = new CreateUserRequest
            {
                Username = "testuser",
                Email = "testuser@example.com",
                FirstName = "Test",
                LastName = "User",
                DateOfBirth = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };

            await _sut.CreateUserAsync(request);

            _publisherMock.Verify(p => p.PublishUserCreatedAsync(It.IsAny<User>()), Times.Once);
        }
    }
}
