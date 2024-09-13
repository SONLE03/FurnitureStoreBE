using System.ComponentModel.DataAnnotations;

namespace FurnitureStoreBE.DTOs.Request.UserRequest
{
    public class UserRequestCreate
    {
        [Required(ErrorMessage = "Full name is required.")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Date Of Birth is required.")]
        public DateTime DateOfBirth { get; set; }
        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^0\d{7,13}$", ErrorMessage = "Phone number must start with '0' and be between 8 and 14 digits long.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression(
       @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@#$%^&*()_+!]).{8,}$",
       ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, one special character, and be at least 8 characters long.")]
        public string Password { get; set; }
        [Required(ErrorMessage = "User claims is required.")]
        public List<int> UserClaimsRequest { get; set; }
    }
    public class UserRequestUpdate
    {
        [Required(ErrorMessage = "Full name is required.")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Date Of Birth is required.")]
        public DateTime DateOfBirth { get; set; }
        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^0\d{7,13}$", ErrorMessage = "Phone number must start with '0' and be between 8 and 14 digits long.")]
        public string PhoneNumber { get; set; }
        
    }
    public class  UserClaimsRequest
    {
        [Required(ErrorMessage = "User claims is required.")]
        public List<int> UserClaims { get; set; }
    }
}
