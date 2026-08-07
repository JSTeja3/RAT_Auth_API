using  RAT_AUTH_API.DTOs.Requests;

namespace RAT_AUTH_API.Interfaces.Services
{
    public interface IAuthService
    {
        Task RegisterUserAsync(RegisterRequest request);
    }
}