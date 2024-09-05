using System.ComponentModel.DataAnnotations;

namespace FurnitureStoreBE.DTOs.Request.AuthRequest
{
    public class OtpRequest
    {
        [Required(ErrorMessage = "Email is required.")]

        public string Email { get; set; }
        [Required(ErrorMessage = "Otp is required.")]

        public int Otp { get; set; }
    }
}
