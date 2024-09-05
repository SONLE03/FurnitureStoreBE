using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureStoreBE.Models
{
    [Table("Reply")]
    public class Reply : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public required string Content { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }

        public Guid? ReviewId { get; set; }
        public Review? Review { get; set; }
        public Guid? QuestionId { get; set; }
        public Question? Question { get; set; }
    }
}
