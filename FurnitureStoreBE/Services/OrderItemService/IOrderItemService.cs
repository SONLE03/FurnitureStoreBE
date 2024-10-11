using FurnitureStoreBE.DTOs.Request.OrderRequest;
using FurnitureStoreBE.DTOs.Response.OrderResponse;

namespace FurnitureStoreBE.Services.CartService
{
    public interface IOrderItemService
    {
        Task<List<OrderItemResponse>> GetCartItemByUser(string userId);
        Task<OrderItemResponse> AddCartItem(OrderItemRequest orderItemRequest, string userId);
        Task RemoveCartItem(Guid orderItemId);
        Task<OrderItemResponse> UpdateCartItemQuantity(Guid orderItemId, long quantity);
    }
}
