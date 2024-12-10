using System.ComponentModel.DataAnnotations;

namespace FurnitureStoreBE.DTOs.Response.UserResponse
{
    public class UserResponse 
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string ImageSource {  get; set; }
        public string Role { get; set; }
        public bool? IsDeleted { get; set; } 
        public bool? IsLocked { get; set; }
    }
}
