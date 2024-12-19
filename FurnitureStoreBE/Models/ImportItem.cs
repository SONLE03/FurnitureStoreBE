using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureStoreBE.Models
{
    [Table("ImportItem")]

    public class ImportItem
    {
        public Guid ImportInvoiceId { get; set; }
        public ImportInvoice ImportInvoice { get; set; }
        public Guid ProductVariantId { get; set; }
        public ProductVariant ProductVariant { get; set; }
        public long Quantity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; } = 0;
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; } = 0;

    }
}
