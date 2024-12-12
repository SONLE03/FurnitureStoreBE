using FurnitureStoreBE.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Principal;

namespace FurnitureStoreBE.Models
{
    [Table("User")]
    public class User : IdentityUser
    {
        [PersonalData]
        public string? FullName { get; set; }
        [PersonalData]
        [Column(TypeName = "date")]
        public DateTime? DateOfBirth { get; set; }
        [PersonalData]
        public Guid? AssetId { get; set; }
        [PersonalData]
        public Asset? Asset { get; set; }
        public bool? IsDeleted { get; set; } = false;
        public bool? IsLocked { get; set; } = false;
        public string Role { get; set; }
        public ICollection<RefreshToken>? Tokens { get; set; }
        public ICollection<Notification>? Notifications { get; set; }

        public ICollection<Review>? Reviews { get; set; }
        public ICollection<Question>? Question { get; set; }
        public ICollection<Reply>? Reply { get; set; }
        public ICollection<UserUsedCoupon>? UserUsedCoupon { get; set; }
        public ICollection<Address>? Addresses { get; set; }
        public ICollection<Order>? Orders { get; set; }
        public ICollection<Favorite>? Favorites { get; set; }
        public ICollection<OrderItem>? OrderItems { get; set; }

        public Cart Cart { get; set; }
    }
}
