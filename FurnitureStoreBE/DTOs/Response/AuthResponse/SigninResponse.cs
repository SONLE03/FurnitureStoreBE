namespace FurnitureStoreBE.DTOs.Response.AuthResponse
{
    public class SigninResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string UserId { get; set; }
        public string Role { get; set; }
    }
}
