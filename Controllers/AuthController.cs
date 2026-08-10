using Microsoft.AspNetCore.Mvc;
using RAT_AUTH_API.Interfaces.Services;
using RAT_AUTH_API.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;

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

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginRequest request)
        {
            var response = await _authService.LoginAsync(request);

            return Ok(response);
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            return Ok(new
            {
                Message = "You are authenticated."
            });
        }

    }

}