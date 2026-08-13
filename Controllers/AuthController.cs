using Microsoft.AspNetCore.Mvc;
using RAT_AUTH_API.Interfaces.Services;
using RAT_AUTH_API.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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


        [HttpPost("refresh")]

        public async Task<IActionResult> RefreshAsync(RefreshRequest request)
        {
            var response = await _authService.RefreshAsync(request);

            return Ok(response);
        }


        [HttpPost("logout")]
        public async Task<IActionResult> LogoutAsync(LogoutRequest request)
        {
            var response = await _authService.LogoutAsync(request);

            return Ok(response);
        }

        [HttpGet("profile")]
        [Authorize]
        public IActionResult GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var email = User.FindFirstValue(ClaimTypes.Email);


            return Ok(new
            {
                UserId = userId,
                Email = email
            });
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult Admin()
        {
            return Ok(new
            {
                Message = "Welcome Admin"
            });
        }

    }

}