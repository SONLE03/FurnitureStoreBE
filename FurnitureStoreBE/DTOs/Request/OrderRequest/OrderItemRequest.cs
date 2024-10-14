namespace FurnitureStoreBE.DTOs.Request.OrderRequest
{
    public class OrderItemRequest
    {
        public Guid ProductId { get; set; }
        public string Dimension { get; set; }
        public Guid ColorId { get; set; }
        public long Quantity { get; set; }
    }
}
