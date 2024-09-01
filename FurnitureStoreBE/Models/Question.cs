using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureStoreBE.Models
{
    [Table("Question")]
    public class Question : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public required string Content { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }

        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        public ICollection<Reply>? Reply { get; set; }
    }
}
