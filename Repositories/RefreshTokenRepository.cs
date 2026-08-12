using RAT_AUTH_API.Models;
using RAT_AUTH_API.Interfaces.Repositories;
using RAT_AUTH_API.Data;
using Microsoft.EntityFrameworkCore;

namespace RAT_AUTH_API.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _dbContext;

        public RefreshTokenRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(RefreshToken refreshToken)
        {
            try
    {
        await _dbContext.RefreshTokens.AddAsync(refreshToken);
        await _dbContext.SaveChangesAsync();
    }
    catch (DbUpdateException ex)
    {
        Console.WriteLine(ex.InnerException?.Message);
        throw;
    }
        }

        public async Task<RefreshToken?> GetTokenHashAsync(string tokenHash)
        {
            return await _dbContext.RefreshTokens.Include(r=>r.User).FirstOrDefaultAsync(r=>r.TokenHash == tokenHash);
        }

        public async Task UpdateAsync(RefreshToken refreshToken)
        {
            _dbContext.RefreshTokens.Update(refreshToken);
            await _dbContext.SaveChangesAsync();
        }
    }
}