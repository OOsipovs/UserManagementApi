using UserManagementApi.Domain.Entities;

namespace UserManagementApi.Application.Interfaces
{
    public interface IMessagePublisher
    {
        Task PublishUserCreatedAsync(User user);
    }
}
