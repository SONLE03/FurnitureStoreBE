using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureStoreBE.Models
{
    [Table("Review")]
    public class Review : BaseEntity    
    {
        [Key]
        public Guid Id { get; set; }

        public ICollection<Asset> Asset { get; set; }
        public required string Content { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }

        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        [Range(1, 5, ErrorMessage = "Rate must be between 1 and 5.")]
        public int Rate {  get; set; }

        public ICollection<Reply>? Reply { get; set; } 
    }
}
