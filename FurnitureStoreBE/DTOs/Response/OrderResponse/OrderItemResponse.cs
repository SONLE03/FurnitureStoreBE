using FurnitureStoreBE.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureStoreBE.DTOs.Response.OrderResponse
{
    public class OrderItemResponse
    {
        public Guid Id { get; set; } 
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string Dimension { get; set; }
        public Guid ColorId { get; set; }
        public string ColorName { get; set; }
        public decimal Price { get; set; }
        public long Quantity { get; set; }
        public decimal SubTotal { get; set; }
    }
}
