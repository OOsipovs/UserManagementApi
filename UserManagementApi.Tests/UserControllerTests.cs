using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UserManagementApi.API.Controllers;
using UserManagementApi.Application.DTOs;
using UserManagementApi.Application.Interfaces;

namespace UserManagementApi.Tests
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _serviceMock = new();
        private readonly UsersController _sut;

        public UserControllerTests()
        {
            _sut = new UsersController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetById_UserExists_ReturnsUser()
        {
            var id = Guid.NewGuid();
            var response = new UserResponse { Id = id, Username = "user", Email = "user@example.com" };
            _serviceMock.Setup(s => s.GetUserByIdAsync(id)).ReturnsAsync(response);

            var result = await _sut.GetById(id);

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            ok.StatusCode.Should().Be(StatusCodes.Status200OK);
            ok.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task GetById_UserNotFound_Returns404()
        {
            _serviceMock.Setup(s => s.GetUserByIdAsync(It.IsAny<Guid>())).ReturnsAsync((UserResponse?)null);

            var result = await _sut.GetById(Guid.NewGuid());

            result.Should().BeOfType<NotFoundResult>()
                  .Which.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [Fact]
        public async Task CreateUser_ValidRequest_ReturnsStatusCreated()
        {
            var id = Guid.NewGuid();
            var request = new CreateUserRequest
            {
                Username = "user",
                Email = "user@example.com",
                FirstName = "User",
                LastName = "Example",
                DateOfBirth = new DateTime(1995, 6, 15, 0, 0, 0, DateTimeKind.Utc)
            };
            var response = new UserResponse { Id = id, Username = "user", Email = "user@example.com" };
            _serviceMock.Setup(s => s.CreateUserAsync(request)).ReturnsAsync(response);

            var result = await _sut.Create(request);

            var created = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            created.StatusCode.Should().Be(StatusCodes.Status201Created);
            created.RouteValues!["id"].Should().Be(id);
            created.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task CreateUser_ValidRequest_CallsServiceExactlyOnce()
        {
            var request = new CreateUserRequest
            {
                Username = "user",
                Email = "user@example.com",
                FirstName = "User",
                LastName = "Example",
                DateOfBirth = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };
            _serviceMock.Setup(s => s.CreateUserAsync(request))
                        .ReturnsAsync(new UserResponse { Id = Guid.NewGuid() });

            await _sut.Create(request);

            _serviceMock.Verify(s => s.CreateUserAsync(request), Times.Once);
        }
    }
}
