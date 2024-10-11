using FurnitureStoreBE.Enums;

namespace FurnitureStoreBE.DTOs.Request.OrderRequest
{
    public class UpdateOrderStatusRequest
    {
        public Guid OrderId { get; set; }
        public EOrderStatus EOrderStatus { get; set; }
        public string? Note { get; set; }   
        public List<IFormFile>? Images { get; set; }
        public string? ShipperId { get; set; }
    }
}
