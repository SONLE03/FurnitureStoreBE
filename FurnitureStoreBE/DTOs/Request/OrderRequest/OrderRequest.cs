using FurnitureStoreBE.DTOs.Response.OrderResponse;
using FurnitureStoreBE.Enums;
using FurnitureStoreBE.Models;

namespace FurnitureStoreBE.DTOs.Request.OrderRequest
{
    public class OrderRequest
    {
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public EPaymentMethod PaymentMethod { get; set; }
        public decimal ShippingFee { get; set; } = 30000;
        public string? Note { get; set; }
        public Guid? CouponId { get; set; }
        public string UserId { get; set; }
        public Guid AddressId { get; set; }
        public decimal TaxFee { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public List<Guid> OrderItems { get; set; }
    }
    public class OrderSearchRequest
    {
     

    }
}
