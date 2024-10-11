using FurnitureStoreBE.Constants;
using FurnitureStoreBE.DTOs.Request.OrderRequest;
using FurnitureStoreBE.Services.CartService;
using FurnitureStoreBE.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FurnitureStoreBE.Controllers.CartController
{
    [ApiController]
    [Route(Routes.CART)]
    public class CartController : ControllerBase
    {
        private readonly IOrderItemService _cartService;
        public CartController(IOrderItemService cartService)
        {
            _cartService = cartService;
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCartItem(string userId)
        {
            return new SuccessfulResponse<object>(await _cartService.GetCartItemByUser(userId), (int)HttpStatusCode.OK, "Get brand successfully").GetResponse();
        }
        [HttpPost("{userId}")]
        public async Task<IActionResult> AddCartItem(string userId, [FromBody] OrderItemRequest orderItemRequest)
        {
            return new SuccessfulResponse<object>(await _cartService.AddCartItem(orderItemRequest, userId), (int)HttpStatusCode.Created, "Cart item added successfully").GetResponse();
        }
        [HttpPut("{cartItemId}")]
        public async Task<IActionResult> UpdateCartItemQuantity(Guid cartItemId, [FromQuery] long quantity)
        {
            return new SuccessfulResponse<object>(await _cartService.UpdateCartItemQuantity(cartItemId, quantity), (int)HttpStatusCode.OK, "Cart item quantity modified successfully").GetResponse();
        }
        [HttpDelete("{cartItemId}")]
        public async Task<IActionResult> DeleteCartItem(Guid cartItemId)
        {
            await _cartService.RemoveCartItem(cartItemId);
            return new SuccessfulResponse<object>(null, (int)HttpStatusCode.OK, "Cart item deleted successfully").GetResponse();

        }
    }
}
