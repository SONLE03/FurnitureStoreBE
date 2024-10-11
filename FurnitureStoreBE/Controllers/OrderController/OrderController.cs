using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.Constants;
using FurnitureStoreBE.DTOs.Request.OrderRequest;
using FurnitureStoreBE.Services.OrderService;
using FurnitureStoreBE.Services.ProductService.BrandService;
using FurnitureStoreBE.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FurnitureStoreBE.Controllers.OrderController
{
    [ApiController]
    [Route(Routes.ORDER)]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpPost("mock")]
        public async Task<IActionResult> CreateMockOrder([FromForm] OrderRequest orderRequest)
        {
            return new SuccessfulResponse<object>(await _orderService.CreateMockOrder(orderRequest), (int)HttpStatusCode.Created, "Your order has been successfully added").GetResponse();
        }

    }
}
