namespace RAT_AUTH_API.DTOs.Requests
{
    public class LogoutRequest
    {
        public string RefreshToken{get; set;}=string.Empty;
    }
}