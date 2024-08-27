using FurnitureStoreBE.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureStoreBE.Models
{
    [Table("User")]
    public class User : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public string? FullName {  get; set; }
        public string? Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public ERole Role { get; set; }
        public Guid? AssetId { get; set; }
        public Asset? Asset { get; set; }
        public ICollection<RefreshToken>? Tokens { get; set; }
        public ICollection<Notification>? Notifications { get; set; }

        public ICollection<Review>? Reviews { get; set; }
        public ICollection<Question>? Question { get; set; }
        public ICollection<Reply>? Reply { get; set; }
        public ICollection<UserUsedCoupon>? UserUsedCoupon { get; set; }
        public ICollection<Address>? Addresses { get; set; }
        public ICollection<Order>? Orders { get; set; }
        public ICollection<Favorite>? Favorites { get; set; }
        public Cart Cart { get; set; }
    }
}
