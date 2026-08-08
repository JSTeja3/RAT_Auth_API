using System.ComponentModel.DataAnnotations;

namespace RAT_AUTH_API.DTOs.Requests
{
    public class RegisterRequest
    {
        [Required]
        [MaxLength(100)]
        public string FirstName{get; set;}=string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName{get; set;}=string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email{get; set;}=string.Empty;

        [Required]
        [MinLength(8)]
        public string Password{get; set;}=string.Empty;
    }
}