using AutoMapper;
using AutoMapper.QueryableExtensions;
using FurnitureStoreBE.Common;
using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.Data;
using FurnitureStoreBE.DTOs.Request.OrderRequest;
using FurnitureStoreBE.DTOs.Response.OrderResponse;
using FurnitureStoreBE.Enums;
using FurnitureStoreBE.Exceptions;
using FurnitureStoreBE.Models;
using FurnitureStoreBE.Services.Caching;
using FurnitureStoreBE.Services.CartService;
using FurnitureStoreBE.Services.CouponService;
using FurnitureStoreBE.Services.FileUploadService;
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
            await _redisCacheService.SetData(order.Id.ToString(), cacheOrder, TimeSpan.FromMinutes(20));
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
        public async Task<OrderResponse> UpdateOrderStatus(Guid orderId, OrderStatusRequest updateOrderStatusRequest)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var order = await _dbContext.Orders.SingleOrDefaultAsync(o => o.Id == orderId);
                if (order == null)
                {
                    throw new ObjectNotFoundException("Order not found");
                }
                order.OrderStatus = updateOrderStatusRequest.EOrderStatus;
                var orderStatus = new OrderStatus
                {
                    Order = order,
                    ShipperId = updateOrderStatusRequest.ShipperId,
                    Status = updateOrderStatusRequest.EOrderStatus,
                    Note = updateOrderStatusRequest.Note,
                };
                if (updateOrderStatusRequest.Images != null)
                {
                    var productVariantImagesUploadResult = await _fileUploadService.UploadFilesAsync(updateOrderStatusRequest.Images, EUploadFileFolder.OrderStatus.ToString());
                    var assets = productVariantImagesUploadResult.Select(img => new Asset
                    {
                        Name = img.OriginalFilename,
                        URL = img.Url.ToString(),
                        CloudinaryId = img.PublicId,
                        FolderName = EUploadFileFolder.OrderStatus.ToString()
                    }).ToList();
                    orderStatus.Asset = assets;
                }
                _dbContext.Orders.Update(order);
                await _dbContext.OrderStatus.AddAsync(orderStatus);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return _mappers.Map<OrderResponse>(order);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
