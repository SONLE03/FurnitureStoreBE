using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureStoreBE.Models
{
    [Table("ImportInvoice")]
    public class ImportInvoice : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; } = 0;
        public ICollection<ImportItem>? ImportItem { get; set; }

    }
}
