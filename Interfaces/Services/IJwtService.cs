using RAT_AUTH_API.Models;

namespace RAT_AUTH_API.Interfaces.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}