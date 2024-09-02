using System.ComponentModel.DataAnnotations;

namespace FurnitureStoreBE.DTOs.Request.AuthRequest
{
    public class RefreshTokenRequest
    {
        [Required(ErrorMessage = "Token is required.")]
        public string Token { get; set; }
        [Required(ErrorMessage = "User is required.")]
        public string UserId { get; set; }
    }
}
