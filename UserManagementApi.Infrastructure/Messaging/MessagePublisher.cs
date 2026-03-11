using Microsoft.Extensions.Logging;
using System.Text.Json;
using UserManagementApi.Domain.Entities;
using UserManagementApi.Domain.Interfaces;

namespace UserManagementApi.Infrastructure.Messaging
{
    public class MessagePublisher : IMessagePublisher
    {
        private readonly ILogger<MessagePublisher> _logger;

        public MessagePublisher(ILogger<MessagePublisher> logger)
        {
            _logger = logger;
        }

        public Task PublishUserCreatedAsync(User user)
        {
            var message = JsonSerializer.Serialize(new
            {
                user.Id,
                user.Username,
                user.Email
            });

            _logger.LogInformation("Publishing message: {Message}", message);
            return Task.CompletedTask;
        }
    }
}
