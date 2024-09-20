using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureStoreBE.Models
{
    [Table("Address")]
    public class Address
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Province { get; set; }
        [Required]
        public string District { get; set; }
        [Required]
        public string Ward { get; set; }
        [Required]
        public  string SpecificAddress { get; set; }
        [Required]
        public string PostalCode { get; set; }
        [Required]
        public bool IsDefault { get; set; }
        [Required]
        public bool IsDeleted { get; set; } = false;

        public string UserId { get; set; }    
        public User User { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
