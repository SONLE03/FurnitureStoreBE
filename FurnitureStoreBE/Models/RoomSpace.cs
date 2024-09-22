using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureStoreBE.Models
{
    [Table("RoomSpace")]
    public class RoomSpace : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public required string RoomSpaceName { get; set; }
        public string? Description { get; set; }

        public Guid? AssetId { get; set; }
        public Asset? Asset { get; set; }

        public ICollection<FurnitureType>? FurnitureTypes { get; set; }
    }
}
