using AutoMapper;
using AutoMapper.QueryableExtensions;
using FurnitureStoreBE.Common;
using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.Data;
using FurnitureStoreBE.DTOs.Request.OrderRequest;
using FurnitureStoreBE.DTOs.Response.OrderResponse;
using FurnitureStoreBE.DTOs.Response.ProductResponse;
using FurnitureStoreBE.DTOs.Response.QuestionResponse;
using FurnitureStoreBE.Enums;
using FurnitureStoreBE.Exceptions;
using FurnitureStoreBE.Models;
using FurnitureStoreBE.Services.Caching;
using FurnitureStoreBE.Services.CartService;
using FurnitureStoreBE.Services.CouponService;
using FurnitureStoreBE.Services.FileUploadService;
using FurnitureStoreBE.Services.OrderService.OrderStatusStrategy;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FurnitureStoreBE.Services.OrderService
{
    public class OrderServiceImp : IOrderService
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly IMapper _mappers;
        private readonly ICouponService _couponService;
        private readonly IRedisCacheService _redisCacheService;
        private readonly IFileUploadService _fileUploadService;
        private readonly IOrderItemService _cartService;
        public OrderServiceImp(ApplicationDBContext dbContext, IMapper mappers, ICouponService couponService, IRedisCacheService redisCacheService
            , IFileUploadService fileUploadService, IOrderItemService cartService)
        {
            _dbContext = dbContext;
            _mappers = mappers;
            _couponService = couponService;
            _redisCacheService = redisCacheService;
            _fileUploadService = fileUploadService;
            _cartService = cartService;
        }
        private async Task<(Order, List<OrderItem>)> GenerateOrderData(OrderRequest orderRequest)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                Coupon coupon = null;
                if (orderRequest.CouponId != null)
                {
                    coupon = await _couponService.UseCoupon((Guid)orderRequest.CouponId);
                }
                var address = await _dbContext.Addresss.FirstOrDefaultAsync(a => a.Id == orderRequest.AddressId);
                if (address == null)
                {
                    throw new ObjectNotFoundException("Address not found");
                }
                var orderItems = await _dbContext.OrderItems.Where(ot => orderRequest.OrderItems.Contains(ot.Id)).ToListAsync();
                var order = new Order
                {
                    PhoneNumber = orderRequest.PhoneNumber,
                    Email = orderRequest.Email,
                    Note = orderRequest.Note,
                    Coupon = coupon,
                    UserId = orderRequest.UserId,
                    Address = address,
                    TaxFee = orderRequest.TaxFee,
                    SubTotal = orderRequest.SubTotal,
                    Total = orderRequest.Total,
                    ShippingFee = orderRequest.ShippingFee,
                    OrderItems = orderItems,
                    PaymentMethod = orderRequest.PaymentMethod,
                    OrderStatus = EOrderStatus.Pending,
                    AccountsReceivable = orderRequest.Total,
                };
                order.setCommonCreate(UserSession.GetUserId());
                foreach (var item in orderItems)
                {
                    item.CartId = null;
                    //item.OrderId = order.Id;
                }
                await transaction.CommitAsync();
                return (order, orderItems);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        private async Task<(Order order, List<OrderItem> orderItems)> RetrieveCachedOrder(Guid orderId)
        {
            var cacheKey = orderId.ToString();
            var cachedData = await _redisCacheService.GetData<CacheOrder>(cacheKey);
            if (cachedData == null)
            {
                throw new Exception("Cached order not found.");
            }
            return (cachedData.Order, cachedData.OrderItems);
        }
        public async Task<OrderResponse> CreateMockOrder(OrderRequest orderRequest)
        {
            var (order, orderItems) = await GenerateOrderData(orderRequest);
            var cacheOrder = new { Order = order, OrderItems = orderItems }; // Create an anonymous object
            await _redisCacheService.SetData(order.Id.ToString(), cacheOrder, TimeSpan.FromMinutes(15));
            return _mappers.Map<OrderResponse>(order);
        }

        public async Task<OrderResponse> CreateOrder(OrderRequest orderRequest)
        {
            var (order, orderItems) = await GenerateOrderData(orderRequest);
            await _dbContext.Orders.AddAsync(order);
            _dbContext.OrderItems.UpdateRange(orderItems);
            await _dbContext.SaveChangesAsync();
            return _mappers.Map<OrderResponse>(order);
        }
        public async Task<OrderResponse> CreateOrderPaid(Guid orderId)
        {
            var (order, orderItems) = await RetrieveCachedOrder(orderId);
            order.OrderStatus = EOrderStatus.Paid;
            order.AccountsReceivable = 0;
            await _dbContext.Orders.AddAsync(order);
            _dbContext.OrderItems.UpdateRange(orderItems);
            await _dbContext.SaveChangesAsync();
            return _mappers.Map<OrderResponse>(order);
        }
        private async Task<PaginatedList<OrderResponse>> GetOrders(PageInfo pageInfo, Expression<Func<Order, bool>> predicate = null)
        {
            predicate ??= r => true; // Nếu predicate là null, sử dụng một điều kiện luôn đúng
            var orderQuery = _dbContext.Orders
                .Include(r => r.OrderItems)
                .Where(predicate)
                .OrderByDescending(c => c.CreatedDate)
                .ProjectTo<OrderResponse>(_mappers.ConfigurationProvider);

            var count = await _dbContext.Orders.CountAsync(predicate);
            return await Task.FromResult(PaginatedList<OrderResponse>.ToPagedList(orderQuery, pageInfo.PageNumber, pageInfo.PageSize));
        }
        public async Task<PaginatedList<OrderResponse>> GetAllOrders(PageInfo pageInfo, OrderSearchRequest orderSearchRequest)
        {
            return await GetOrders(pageInfo);
        }

        public async Task<PaginatedList<OrderResponse>> GetAllOrdersByCustomer(PageInfo pageInfo, OrderSearchRequest orderSearchRequest, string userId)
        {
            return await GetOrders(pageInfo, (r => r.UserId == userId && !r.IsDeleted));
        }  
        public async Task<OrderResponse> UpdateOrderStatus(Guid orderId, UpdateOrderStatusRequest updateOrderStatusRequest)
        {
            var order = await _dbContext.Orders.SingleOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
            {
                throw new ObjectNotFoundException("Order not found");
            }
            OrderActionManager orderActionManager = new OrderActionManager();
            switch (updateOrderStatusRequest.EOrderStatus)
            {
                case EOrderStatus.Canceled:
                    orderActionManager.setActionStrategy(new CancelOrderStrategy(_dbContext, _fileUploadService, _cartService, _mappers));
                    break;
                case EOrderStatus.DeliveryToShipper:
                    orderActionManager.setActionStrategy(new DeliverOrderToShipperStrategy(_dbContext, _fileUploadService, _mappers));
                    break;
                case EOrderStatus.Delivering:
                    orderActionManager.setActionStrategy(new DeliverOrderToCustomerStrategy(_dbContext, _fileUploadService, _mappers));
                    break;
                case EOrderStatus.Completed:
                    orderActionManager.setActionStrategy(new CompleteOrderStrategy(_dbContext, _fileUploadService, _mappers));
                    break;
                case EOrderStatus.ReturnGoods:
                    orderActionManager.setActionStrategy(new ReturnOrderStrategy(_dbContext, _fileUploadService, _mappers));
                    break;
                default:
                    throw new BusinessException("Order status not found");
            }
            return await orderActionManager.executeAction(order,updateOrderStatusRequest);
        }
    }
}
