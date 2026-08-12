using RAT_AUTH_API.Models;

namespace RAT_AUTH_API.Interfaces.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken refreshtoken);

        Task<RefreshToken?> GetTokenHashAsync(string tokenHash);

        Task UpdateAsync(RefreshToken refreshToken);
    }
}