using AutoMapper;
using FurnitureStoreBE.Common;
using FurnitureStoreBE.Data;
using FurnitureStoreBE.DTOs.Request.OrderRequest;
using FurnitureStoreBE.DTOs.Response.OrderResponse;
using FurnitureStoreBE.Enums;
using FurnitureStoreBE.Models;
using FurnitureStoreBE.Services.CartService;
using FurnitureStoreBE.Services.FileUploadService;

namespace FurnitureStoreBE.Services.OrderService.OrderStatusStrategy
{
    public class CancelOrderStrategy : OrderActionStrategy
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly IFileUploadService _fileUploadService;
        private readonly IOrderItemService _cartService;
        private readonly IMapper _mappers;
        public CancelOrderStrategy(ApplicationDBContext dbContext, IFileUploadService fileUploadService, IOrderItemService cartService, IMapper mappers)
        {
            _dbContext = dbContext;
            _fileUploadService = fileUploadService;
            _cartService = cartService;
            _mappers = mappers;
        }
        public async Task<OrderResponse> UpdateOrderStatus(Order order, UpdateOrderStatusRequest updateOrderStatusRequest)
        {
            var shipperId = updateOrderStatusRequest.ShipperId;
            var orderStatusRecord = new OrderStatus
            {
                OrderId = order.Id,
                Status = EOrderStatus.Canceled,
                Note = updateOrderStatusRequest.Note,
                ShipperId = shipperId
            };
            orderStatusRecord.setCommonCreate(UserSession.GetUserId());
            if (updateOrderStatusRequest.Images != null)
            {
                var productVariantImagesUploadResult = await _fileUploadService.UploadFilesAsync(updateOrderStatusRequest.Images, EUploadFileFolder.Product.ToString());
                var assets = productVariantImagesUploadResult.Select(img => new Asset
                {
                    Name = img.OriginalFilename,
                    URL = img.Url.ToString(),
                    CloudinaryId = img.PublicId,
                    FolderName = EUploadFileFolder.Product.ToString()
                }).ToList();
                orderStatusRecord.Asset = assets;
            }
            var orderItem = order.OrderItems.ToList();
            foreach (var item in orderItem)
            {
                await _cartService.RemoveCartItem(item.Id);
            }
            await _dbContext.OrderStatus.AddAsync(orderStatusRecord);
            await _dbContext.SaveChangesAsync();
            return _mappers.Map<OrderResponse>(order);
        }
    }
}
