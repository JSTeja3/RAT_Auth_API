using RAT_AUTH_API.DTOs.Requests;
using RAT_AUTH_API.DTOs.Responses;
using RAT_AUTH_API.Models;
using RAT_AUTH_API.Interfaces.Repositories;
using RAT_AUTH_API.Interfaces.Services;
using RAT_AUTH_API.Exceptions;
using RAT_AUTH_API.Data;
using Superpower.Parsers;


namespace RAT_AUTH_API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IJwtService _jwtService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepo;
        private readonly AppDbContext _dbContext;

        public AuthService(IUserRepository userRepo, IJwtService jwtService, IRefreshTokenService refreshTokenService, IRefreshTokenRepository refreshTokenRepo, AppDbContext dbContext)
        {
            _userRepo = userRepo;
            _jwtService = jwtService;
            _refreshTokenService = refreshTokenService;
            _refreshTokenRepo = refreshTokenRepo;
            _dbContext = dbContext;
        }
        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            var email = request.Email.Trim().ToLowerInvariant();
            var emailExists = await _userRepo.EmailExistsAsync(email);

            if (emailExists)
            {
                throw new DuplicateEmailException("Email already exists");
            }

            User user = new User
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(request.Password)
            };

            await _userRepo.RegisterAsync(user);

            return new RegisterResponse
            {
                Id = user.Id,
                Message = "User registered successfully"
            };
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var email = request.Email.Trim().ToLowerInvariant();
            var user = await _userRepo.GetByEmailAsync(email);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            var passwordValid = BCrypt.Net.BCrypt.EnhancedVerify(request.Password, user.PasswordHash);

            if (!passwordValid)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            string accesstoken = _jwtService.GenerateToken(user);

            string refreshToken = _refreshTokenService.GenerateToken();
            string refreshTokenHash = _refreshTokenService.HashToken(refreshToken);

            var refreshTokenDb = new RefreshToken
            {
                TokenHash = refreshTokenHash,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                UserId = user.Id
            };

            await _refreshTokenRepo.AddAsync(refreshTokenDb);

            await _dbContext.SaveChangesAsync();


            return new LoginResponse
            {
                AccessToken = accesstoken,
                RefreshToken = refreshToken
            };

        }
        public async Task<LoginResponse> RefreshAsync(RefreshRequest request)
        {
            var tokenHash = _refreshTokenService.HashToken(request.RefreshToken);

            var storedToken = await _refreshTokenRepo.GetTokenHashAsync(tokenHash);

            if (storedToken is null)
            {
                throw new UnauthorizedAccessException("Invalid refresh token.");
            }

            if (storedToken.RevokedAt is not null)
            {
                throw new UnauthorizedAccessException("Refresh token has been revoked.");
            }

            if (storedToken.ExpiresAt <= DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Refresh token has expired.");
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try{
            var user = storedToken.User;

            // Revoke old refresh token
            storedToken.RevokedAt = DateTime.UtcNow;

            await _refreshTokenRepo.UpdateAsync(storedToken);

            // Generate new tokens
            var newAccessToken = _jwtService.GenerateToken(user);
            var newRefreshToken = _refreshTokenService.GenerateToken();

            var newRefreshTokenHash = _refreshTokenService.HashToken(newRefreshToken);

            var newRefreshTokenDb = new RefreshToken
            {
                TokenHash = newRefreshTokenHash,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                UserId = user.Id
            };

            await _refreshTokenRepo.AddAsync(newRefreshTokenDb);

            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();

            return new LoginResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

        }

        public async Task<LogoutResponse> LogoutAsync(LogoutRequest request)
        {
            var tokenHash = _refreshTokenService.HashToken(request.RefreshToken);

            var storedToken = await _refreshTokenRepo.GetTokenHashAsync(tokenHash);

            if(storedToken is null)
            {
                throw new UnauthorizedAccessException("Invlaid refresh token.");
            }
            if(storedToken.RevokedAt is not null)
            {
                return new LogoutResponse
                {
                  Message = "Logged out successfully."  
                };
            }

            storedToken.RevokedAt = DateTime.UtcNow;

            await _refreshTokenRepo.UpdateAsync(storedToken);

            await _dbContext.SaveChangesAsync();

            return new LogoutResponse
                {
                  Message = "Logged out successfully."  
                };
        }
    }
}