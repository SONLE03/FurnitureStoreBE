using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureStoreBE.Models
{
    [Table("Material")]
    public class Material : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public required string MaterialName { get; set; }
        public string? Description { get; set; }

        public Guid? AssetId { get; set; }
        public Asset? Asset { get; set; }
        public ICollection<Product>? Products { get; set; } 
    }
}
