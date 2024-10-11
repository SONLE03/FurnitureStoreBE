using FurnitureStoreBE.DTOs.Request.OrderRequest;
using FurnitureStoreBE.DTOs.Response.OrderResponse;
using FurnitureStoreBE.Models;

namespace FurnitureStoreBE.Services.OrderService.OrderStatusStrategy
{
    public class OrderActionManager
    {
        private OrderActionStrategy actionStrategy;

        public void setActionStrategy(OrderActionStrategy actionStrategy)
        {
            this.actionStrategy = actionStrategy;
        }

        public async Task<OrderResponse> executeAction(Order order, UpdateOrderStatusRequest updateOrderStatusRequest)
        {
            return await actionStrategy.UpdateOrderStatus(order, updateOrderStatusRequest);
        }
    }
}
