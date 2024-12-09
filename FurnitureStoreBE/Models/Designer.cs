using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureStoreBE.Models
{
    [Table("Designer")]
    public class Designer : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public required string DesignerName { get; set; }   
        public string? Description { get; set; }
        public Guid? AssetId { get; set; }
        public Asset? Asset { get; set; }

        public ICollection<Product>? Products { get; set; }
    }
}
