using RAT_AUTH_API.DTOs.Requests;
using RAT_AUTH_API.DTOs.Responses;
using RAT_AUTH_API.Models;
using RAT_AUTH_API.Interfaces.Repositories;
using RAT_AUTH_API.Interfaces.Services;
using RAT_AUTH_API.Exceptions;


namespace RAT_AUTH_API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IJwtService _jwtService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepo;

        public AuthService(IUserRepository userRepo, IJwtService jwtService, IRefreshTokenService refreshTokenService, IRefreshTokenRepository refreshTokenRepo)
        {
            _userRepo = userRepo;
            _jwtService = jwtService;
            _refreshTokenService = refreshTokenService;
            _refreshTokenRepo = refreshTokenRepo;
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

            return new LoginResponse
            {
              AccessToken = accesstoken,
              RefreshToken = refreshToken   
            };

        }
    }
}