using System.ComponentModel.DataAnnotations;

namespace FurnitureStoreBE.DTOs.Request
{
    public class RegisterRequest
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
