namespace FurnitureStoreBE.DTOs.Request.ImportRequest
{
    public class ImportRequest
    {
        public Guid ProductVariantId { get; set; }
        public long Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
    }
}
