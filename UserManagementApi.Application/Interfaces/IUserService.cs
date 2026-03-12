using UserManagementApi.Application.DTOs;

namespace UserManagementApi.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserResponse?> GetUserByIdAsync(Guid id);
        Task<UserResponse> CreateUserAsync(CreateUserRequest request);
    }
}
