using  RAT_AUTH_API.DTOs.Requests;
using  RAT_AUTH_API.Models;
using RAT_AUTH_API.Interfaces.Repositories;
using RAT_AUTH_API.Interfaces.Services;


namespace RAT_AUTH_API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;

        public AuthService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }
        public async Task RegisterUserAsync(RegisterRequest request)
        {
            var emailExists = await _userRepo.EmailExistsAsync(request.Email);

            if (emailExists)
            {
                throw new Exception("Email already exists");
            }

            User user = new User
            {
              FirstName = request.FirstName,
              LastName = request.LastName,
              Email = request.Email,
              PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(request.Password)
            };

            await _userRepo.AddAsync(user);
        }
    }
}