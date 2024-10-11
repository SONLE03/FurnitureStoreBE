using AutoMapper;
using FurnitureStoreBE.Common;
using FurnitureStoreBE.Data;
using FurnitureStoreBE.DTOs.Request.OrderRequest;
using FurnitureStoreBE.DTOs.Response.OrderResponse;
using FurnitureStoreBE.Enums;
using FurnitureStoreBE.Exceptions;
using FurnitureStoreBE.Models;
using FurnitureStoreBE.Services.FileUploadService;
using Microsoft.EntityFrameworkCore;

namespace FurnitureStoreBE.Services.OrderService.OrderStatusStrategy
{
    public class CompleteOrderStrategy : OrderActionStrategy
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly IFileUploadService _fileUploadService;
        private readonly IMapper _mappers;
        public CompleteOrderStrategy(ApplicationDBContext dbContext, IFileUploadService fileUploadService, IMapper mappers)
        {
            _dbContext = dbContext;
            _fileUploadService = fileUploadService;
            _mappers = mappers;
        }
        public async Task<OrderResponse> UpdateOrderStatus(Order order, UpdateOrderStatusRequest updateOrderStatusRequest)
        {
            var shipperId = updateOrderStatusRequest.ShipperId;
            var orderStatusRecord = new OrderStatus
            {
                OrderId = order.Id,
                Status = EOrderStatus.Completed,
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
            await _dbContext.OrderStatus.AddAsync(orderStatusRecord);
            await _dbContext.SaveChangesAsync();
            return _mappers.Map<OrderResponse>(order);
        }
    }
}
