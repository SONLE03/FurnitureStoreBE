using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureStoreBE.Models
{
    [Table("Cart")]
    public class Cart
    {
        [Key]
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public ICollection<OrderItem>? OrderItems { get; set; }

    }
}
