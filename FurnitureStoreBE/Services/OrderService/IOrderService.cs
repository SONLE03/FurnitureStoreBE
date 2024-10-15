using FurnitureStoreBE.Common;
using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.DTOs.Request.OrderRequest;
using FurnitureStoreBE.DTOs.Response.OrderResponse;
using FurnitureStoreBE.Enums;
using FurnitureStoreBE.Models;

namespace FurnitureStoreBE.Services.OrderService
{
    public interface IOrderService
    {
        Task<PaginatedList<OrderResponse>> GetAllOrders(PageInfo pageInfo, OrderSearchRequest orderSearchRequest);
        Task<PaginatedList<OrderResponse>> GetAllOrdersByCustomer(PageInfo pageInfo, OrderSearchRequest orderSearchRequest, string userId);
        Task<OrderResponse> CreateOrder(OrderRequest orderRequest);
        Task<OrderResponse> CreateOrderPaid(Guid orderId);
        Task<OrderResponse> CreateMockOrder(OrderRequest orderRequest);
        Task<OrderResponse> UpdateOrderStatus(Guid orderId, OrderStatusRequest updateOrderStatusRequest);
    }
}
