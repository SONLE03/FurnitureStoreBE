namespace FurnitureStoreBE.DTOs.Request.ProductRequest
{
    public class ProductDiscountRequest
    {
        public Guid ProductId { get; set; }
        public decimal DiscountValue { get; set; }  
    }
}
