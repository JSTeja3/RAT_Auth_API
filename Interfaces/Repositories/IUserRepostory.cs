using RAT_AUTH_API.Models;

namespace RAT_AUTH_API.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<bool> EmailExistsAsync(string email);
        Task RegisterAsync(User user);
    }
    
}