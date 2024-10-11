using FurnitureStoreBE.DTOs.Request.OrderRequest;
using FurnitureStoreBE.DTOs.Response.OrderResponse;
using FurnitureStoreBE.Models;

namespace FurnitureStoreBE.Services.OrderService.OrderStatusStrategy
{
    public interface OrderActionStrategy
    {
        Task<OrderResponse> UpdateOrderStatus(Order order, UpdateOrderStatusRequest updateOrderStatusRequest);
    }
}
