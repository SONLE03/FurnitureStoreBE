using FurnitureStoreBE.Models;

namespace FurnitureStoreBE.DTOs.Response.ImportResponse
{
    public class ImportResponse
    {
        public Guid Id { get; set; }
        public decimal Total { get; set; } = 0;
        public ICollection<ImportItemResponse> ImportItemResponse { get; set; }

    }
    public class ImportItemResponse
    {
        public Guid ProductVariantId { get; set; }
        public string ProductName { get; set; }
        public long Quantity { get; set; }
        public decimal Price { get; set; } = 0;
        public decimal Total { get; set; } = 0;
    }
}
