using FurnitureStoreBE.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureStoreBE.Models
{
    [Table("Notification")]
    public class Notification : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public required string Content { get; set; }
        public required string Title { get; set; }
        public required string RedirectUrl { get; set; }
        public required bool Read { get; set; } = false;
        public ICollection<User> Users { get; set; }
        public ENotificationType ENotificationType { get; set; }

    }
}
