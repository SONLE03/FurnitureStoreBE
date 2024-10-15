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
        [HttpGet()]
        public async Task<IActionResult> GetAllOrders([FromQuery] PageInfo pageInfo, [FromQuery] OrderSearchRequest orderSearchRequest)
        {
            return new SuccessfulResponse<object>(await _orderService.GetAllOrders(pageInfo, orderSearchRequest), (int)HttpStatusCode.OK, "Get orders successfully").GetResponse();
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetAllOrdersByCustomer([FromQuery] PageInfo pageInfo, [FromQuery] OrderSearchRequest orderSearchRequest, string userId)
        {
            return new SuccessfulResponse<object>(await _orderService.GetAllOrdersByCustomer(pageInfo, orderSearchRequest, userId), (int)HttpStatusCode.OK, "Get orders by customer successfully").GetResponse();
        }
        [HttpPost()]
        public async Task<IActionResult> Createorder([FromForm] OrderRequest orderRequest)
        {
            return new SuccessfulResponse<object>(await _orderService.CreateOrder(orderRequest), (int)HttpStatusCode.Created, "Your order has been successfully added").GetResponse();
        }
        [HttpPost("mock")]
        public async Task<IActionResult> CreateMockOrder([FromForm] OrderRequest orderRequest)
        {
            return new SuccessfulResponse<object>(await _orderService.CreateMockOrder(orderRequest), (int)HttpStatusCode.Created, "Your order has been successfully added").GetResponse();
        }
        [HttpPut("{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(Guid orderId, [FromForm] OrderStatusRequest orderStatusRequest)
        {
            return new SuccessfulResponse<object>(await _orderService.UpdateOrderStatus(orderId, orderStatusRequest), (int)HttpStatusCode.OK, "Order status has been successfully modified").GetResponse();
        }
    }
}
