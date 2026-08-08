using Microsoft.AspNetCore.Mvc;
using RAT_AUTH_API.Interfaces.Services;
using  RAT_AUTH_API.DTOs.Requests;

namespace RAT_AUTH_API.Controllers
{

    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterRequest request)
        {
            var response = await _authService.RegisterAsync(request);

            return StatusCode(StatusCodes.Status201Created, response);
        }
        
    }
    
}