using FurnitureStoreBE.DTOs.Request.OrderRequest;
using FurnitureStoreBE.DTOs.Response.OrderResponse;

namespace FurnitureStoreBE.Services.CartService
{
    public interface ICartService
    {
        Task<List<OrderItemResponse>> GetCartItemByUser(string userId);
        Task<OrderItemResponse> AddCartItem(OrderItemRequest orderItemRequest);
        Task RemoveCartItem(Guid orderItemId);
        Task<OrderItemResponse> UpdateCartItemQuantity(Guid orderItemId, long quantity);
    }
}
