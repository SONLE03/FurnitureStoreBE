using FurnitureStoreBE.DTOs.Response.UserResponse;
using FurnitureStoreBE.Enums;

namespace FurnitureStoreBE.DTOs.Response.OrderResponse
{
    public class OrderResponse
    {
        public Guid Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string PaymentMethod { get; set; } 
        public DateTime CanceledAt { get; set; }
        public DateTime CompletedAt { get; set; }
        public DateTime DeliveredAt { get; set; }
        public string? Note { get; set; }
        public decimal ShippingFee { get; set; } = 30000;
        public string OrderStatus { get; set; } = EOrderStatus.Pending.ToString();

        public string UserId { get; set; }
        public string FullName {  get; set; }
        public AddressResponse Address { get; set; }
        public decimal TaxFee { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public decimal AccountsReceivable { get; set; } = 0;
        public List<OrderItemResponse> OrderItemResponses { get; set; }

    }
}
