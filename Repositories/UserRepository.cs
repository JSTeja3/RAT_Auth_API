using RAT_AUTH_API.Models;
using RAT_AUTH_API.Interfaces.Repositories;
using RAT_AUTH_API.Data;
using Microsoft.EntityFrameworkCore;

namespace RAT_AUTH_API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;
        public UserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbContext.Users.AnyAsync(u=>u.Email == email);
        }

        public async Task RegisterAsync(User user)
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
        }
         
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u=>u.Email == email);
        }
    }
}