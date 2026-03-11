using UserManagementApi.Domain.Entities;

namespace UserManagementApi.Domain.Interfaces
{
    public interface IMessagePublisher
    {
        Task PublishUserCreatedAsync(User user);
    }
}
