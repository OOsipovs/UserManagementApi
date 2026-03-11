using Microsoft.EntityFrameworkCore;
using UserManagementApi.Application.Interfaces;
using UserManagementApi.Domain.Entities;

namespace UserManagementApi.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users.Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Id == id);
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
