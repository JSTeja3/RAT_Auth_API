using RAT_AUTH_API.Interfaces.Services;
using RAT_AUTH_API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace RAT_AUTH_API.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }
        public string GenerateToken(User user)
        {
            var key = _config["JWT_KEY"] ?? throw new InvalidOperationException("JWT key is not configured.");
            var issuer = _config["JWT_ISSUER"] ?? throw new InvalidOperationException("JWT issuer is not configured.");
            var audience = _config["JWT_AUDIENCE"] ?? throw new InvalidOperationException("JWT audience is not configured.");
            var expirationMinutes = int.Parse(_config["JWT_EXPIRATION_MINUTES"] ?? "15");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            var credentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}