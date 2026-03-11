using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagementApi.Application.DTOs;
using UserManagementApi.Application.Interfaces;
using UserManagementApi.Domain.Entities;
using UserManagementApi.Domain.Interfaces;

namespace UserManagementApi.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessagePublisher _messagePublisher;

        public UserService(IUserRepository userRepository, IMessagePublisher messagePublisher)
        {
            _userRepository = userRepository;
            _messagePublisher = messagePublisher;
        }

        public async Task<UserResponse?> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            return new UserResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.Profile?.FirstName ?? string.Empty,
                LastName = user.Profile?.LastName ?? string.Empty,
                DateOfBirth = user.Profile?.DateOfBirth ?? default
            };
        }
        public async Task<UserResponse> CreateUserAsync(CreateUserRequest request)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                Email = request.Email,
                Profile = new Profile
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    DateOfBirth = request.DateOfBirth
                }
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            // Publish an event to RabbitMQ
            await _messagePublisher.PublishUserCreatedAsync(user);

            return new UserResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.Profile?.FirstName ?? string.Empty,
                LastName = user.Profile?.LastName ?? string.Empty,
                DateOfBirth = user.Profile?.DateOfBirth ?? default
            };
        }
    }
}
