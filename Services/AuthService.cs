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

        public AuthService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
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
    }
}