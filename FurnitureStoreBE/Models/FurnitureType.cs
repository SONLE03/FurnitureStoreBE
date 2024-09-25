using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureStoreBE.Models
{
    [Table("FurnitureType")]
    public class FurnitureType : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public required string FurnitureTypeName { get; set; }
        public string? Description { get; set; }

        public Guid? AssetId { get; set; }
        public Asset? Asset { get; set; }
        public Guid RoomSpaceId { get; set; }
        public RoomSpace RoomSpace { get; set; }
        public ICollection<Category>? Categories { get; set; }
    }
}
