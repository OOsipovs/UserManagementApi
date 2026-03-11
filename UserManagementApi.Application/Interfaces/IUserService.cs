using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagementApi.Application.DTOs;

namespace UserManagementApi.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserResponse?> GetUserByIdAsync(Guid id);
        Task<UserResponse> CreateUserAsync(CreateUserRequest request);
    }
}
