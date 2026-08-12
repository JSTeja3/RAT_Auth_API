namespace RAT_AUTH_API.DTOs.Responses
{
    public class LoginResponse
    {
        public string AccessToken{get; set;}=string.Empty;
        public string RefreshToken{get; set;}=string.Empty;
    }
}