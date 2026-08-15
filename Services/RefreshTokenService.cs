using RAT_AUTH_API.Interfaces.Services;
using System.Security.Cryptography;
using System.Text;

namespace RAT_AUTH_API.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        public string GenerateToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);

            return Convert.ToBase64String(randomBytes);
        }

        public string HashToken(string token)
        {
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(token));

            return Convert.ToBase64String(hash);
        }
    }
}