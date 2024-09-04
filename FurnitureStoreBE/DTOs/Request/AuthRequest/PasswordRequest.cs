using System.ComponentModel.DataAnnotations;

namespace FurnitureStoreBE.DTOs.Request.AuthRequest
{
    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "UserId is required.")]
        public string UserId { get; set; }
        [Required(ErrorMessage = "Old password is required.")]
        public string OldPassword { get; set; }
        [Required(ErrorMessage = "New password is required.")]
        public string NewPassword { get; set; }
    }
    public class ResetPasswordRequest
    {
        [Required(ErrorMessage = "UserId is required.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "New password is required.")]
        public string NewPassword { get; set; }
    }
}
