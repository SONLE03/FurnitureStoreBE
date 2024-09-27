using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.Constants;
using FurnitureStoreBE.DTOs.Request.BrandRequest;
using FurnitureStoreBE.DTOs.Request.OrderRequest;
using FurnitureStoreBE.Services.CartService;
using FurnitureStoreBE.Services.ProductService.BrandService;
using FurnitureStoreBE.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FurnitureStoreBE.Controllers.CartController
{
    [ApiController]
    [Route(Routes.CART)]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        //[HttpGet()]
        //public async Task<IActionResult> GetBrands([FromQuery] PageInfo pageInfo)
        //{
        //    return new SuccessfulResponse<object>(await _brandService.GetAllBrands(pageInfo), (int)HttpStatusCode.OK, "Get brand successfully").GetResponse();
        //}
        [HttpPost()]
        public async Task<IActionResult> AddOrderItem([FromBody] OrderItemRequest orderItemRequest)
        {
            return new SuccessfulResponse<object>(await _cartService.AddOrderItem(orderItemRequest), (int)HttpStatusCode.Created, "Order item added successfully").GetResponse();
        }
        [HttpPut("{orderItemId}")]
        public async Task<IActionResult> UpdateOrderItemQuantity(Guid orderItemId, [FromQuery] long quantity)
        {
            return new SuccessfulResponse<object>(await _cartService.UpdateOrderItemQuantity(orderItemId, quantity), (int)HttpStatusCode.OK, "Order item quantity modified successfully").GetResponse();
        }
        [HttpDelete("{orderItemId}")]
        public async Task<IActionResult> DeleteOrderItem(Guid orderItemId)
        {
            await _cartService.RemoveOrderItem(orderItemId);
            return new SuccessfulResponse<object>(null, (int)HttpStatusCode.OK, "Order item deleted successfully").GetResponse();

        }
    }
}
