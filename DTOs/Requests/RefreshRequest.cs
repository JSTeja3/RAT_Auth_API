using System.ComponentModel.DataAnnotations;
namespace RAT_AUTH_API.DTOs.Requests
{
    public class RefreshRequest
    {
        [Required]
        public string RefreshToken{get; set;}=string.Empty;
    }
}