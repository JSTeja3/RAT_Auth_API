using  RAT_AUTH_API.DTOs.Requests;
using  RAT_AUTH_API.DTOs.Responses;

namespace RAT_AUTH_API.Interfaces.Services
{
    public interface IAuthService
    {
        Task<RegisterResponse> RegisterAsync(RegisterRequest request);

        Task<LoginResponse> LoginAsync(LoginRequest request);
    }
}