using FurnitureStoreBE.DTOs.Request.OrderRequest;
using FurnitureStoreBE.DTOs.Response.OrderResponse;

namespace FurnitureStoreBE.Services.CartService
{
    public interface ICartService
    {
        Task<OrderItemResponse> AddOrderItem(OrderItemRequest orderItemRequest);
        Task RemoveOrderItem(Guid orderItemId);
        Task<OrderItemResponse> UpdateOrderItemQuantity(Guid orderItemId, long quantity);
    }
}
