using System.ComponentModel.DataAnnotations;

namespace FurnitureStoreBE.DTOs.Request.Auth
{
    public class SignupRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression(@"^.*@gmail\.com$", ErrorMessage = "Email must end with @gmail.com")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression(
        @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@#$%^&*()_+!]).{8,}$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, one special character, and be at least 8 characters long.")]
        public string Password { get; set; }
    }
}
