
namespace RAT_AUTH_API.Interfaces.Services
{
    public interface IRefreshTokenService
    {
        string GenerateToken();

        string HashToken(string token);
    }
}