using FurnitureStoreBE.Data;
using FurnitureStoreBE.DTOs.Request.OrderRequest;
using FurnitureStoreBE.DTOs.Response.OrderResponse;
using FurnitureStoreBE.Exceptions;
using FurnitureStoreBE.Models;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Drawing;
using System.Security.AccessControl;

namespace FurnitureStoreBE.Services.CartService
{
    public class CartServiceImp : ICartService
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly ILogger<CartServiceImp> _logger;
        public CartServiceImp(ApplicationDBContext dbContext, ILogger<CartServiceImp> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        public async Task<OrderItemResponse> AddOrderItem(OrderItemRequest orderItemRequest)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var product = await _dbContext.Products.Where(p => p.Id == orderItemRequest.ProductId).SingleOrDefaultAsync();
                if (product == null)
                {
                    throw new ObjectNotFoundException("Product not found");
                }
                var userId = orderItemRequest.UserId;
                var colorId = orderItemRequest.ColorId;
                var dimension = orderItemRequest.Dimension;
                var productId = orderItemRequest.ProductId;
                var quantity = orderItemRequest.Quantity;
                var color = await _dbContext.Colors.Where(c => c.Id == colorId).SingleOrDefaultAsync();
                if(color == null)
                {
                    throw new ObjectNotFoundException("Color not found");
                }
                var existOrderItem = await _dbContext.OrderItems
                    .Where(ot => ot.UserId == userId
                            && ot.ColorId == colorId
                            && ot.Dimension.Equals(dimension)
                            && ot.ProductId == productId)
                    .SingleOrDefaultAsync();
                if (existOrderItem != null)
                {
                    transaction.Commit();

                    return await UpdateOrderItemQuantity(existOrderItem.Id, quantity);
                }

                var productVariantIndex = await _dbContext.ProductVariants.Where(pv => pv.ProductId == product.Id && pv.ColorId == colorId && pv.DisplayDimension.Equals(dimension)).SingleOrDefaultAsync();
                if (productVariantIndex == null)
                {
                    throw new ObjectNotFoundException("Product variant not found");
                }
                if (productVariantIndex.Quantity < quantity)
                {
                    throw new BusinessException("Product variant not enough");
                }

                productVariantIndex.Quantity -= quantity;

                var price = productVariantIndex.Price;
                var subtotal = price * quantity;
                if(product.Discount != 0)
                {
                    subtotal = subtotal * product.Discount / 100;
                }
                var orderItem = new OrderItem
                {
                    ProductId = productId,
                    ColorId = colorId,
                    Dimension = dimension,
                    Quantity = quantity,
                    Price = productVariantIndex.Price,
                    SubTotal = subtotal,
                    UserId = userId
                };
                await _dbContext.OrderItems.AddAsync(orderItem);
                _dbContext.ProductVariants.Update(productVariantIndex);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return new OrderItemResponse
                {
                    Id = orderItem.Id,
                    ColorId = colorId,
                    ColorName = color.ColorName,
                    ProductId = productId,
                    ProductName = product.ProductName,
                    Dimension = dimension,
                    Price = price,
                    SubTotal = subtotal,
                    Quantity = quantity
                };
            }
            catch 
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task RemoveOrderItem(Guid orderItemId)
        {
            var orderItem = await _dbContext.OrderItems
                  .Where(ot => ot.Id == orderItemId)
                  .SingleOrDefaultAsync();
            if (orderItem == null)
            {
                throw new ObjectNotFoundException("Order item not found");
            }
            if (!await _dbContext.Products.AnyAsync(p => p.Id == orderItem.ProductId))
            {
                throw new ObjectNotFoundException("Product not found");
            }
           
            var productVariantIndex = await _dbContext.ProductVariants
                .Where(pv => pv.ProductId == orderItem.ProductId && pv.ColorId == orderItem.ColorId && pv.DisplayDimension.Equals(orderItem.Dimension))
                .SingleOrDefaultAsync();
            if (productVariantIndex == null)
            {
                throw new ObjectNotFoundException("Product variant not found");
            }
            productVariantIndex.Quantity += orderItem.Quantity;
            _dbContext.ProductVariants.Update(productVariantIndex);
            _dbContext.OrderItems.Remove(orderItem);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<OrderItemResponse> UpdateOrderItemQuantity(Guid orderItemId, long quantity)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                _logger.LogInformation(quantity.ToString());

                var existOrderItem = await _dbContext.OrderItems
                       .Where(ot => ot.Id == orderItemId)
                       .SingleOrDefaultAsync();

                if (existOrderItem == null)
                {
                    throw new ObjectNotFoundException("Order item not found");
                }
                if (!await _dbContext.Products.AnyAsync(p => p.Id == existOrderItem.ProductId))
                {
                    throw new ObjectNotFoundException("Product not found");
                }
                var color = await _dbContext.Colors.Where(c => c.Id == existOrderItem.ColorId).SingleOrDefaultAsync();
                if (color == null)
                {
                    throw new ObjectNotFoundException("Color not found");
                }
                var product = await _dbContext.Products.Where(p => p.Id == existOrderItem.ProductId).SingleOrDefaultAsync();

                var productVariantIndex = await _dbContext.ProductVariants
                    .Where(pv => pv.ProductId == existOrderItem.ProductId && pv.ColorId == existOrderItem.ColorId && pv.DisplayDimension.Equals(existOrderItem.Dimension))
                    .SingleOrDefaultAsync();
                if (productVariantIndex == null)
                {
                    throw new ObjectNotFoundException("Product variant not found");
                }
                _logger.LogInformation(productVariantIndex.Quantity.ToString());

                productVariantIndex.Quantity += existOrderItem.Quantity;
                _logger.LogInformation(productVariantIndex.Quantity.ToString());

                if (productVariantIndex.Quantity < quantity)
                {
                    throw new BusinessException("Product variant not enough");
                }

                productVariantIndex.Quantity -= quantity;

                var price = productVariantIndex.Price;
                var subtotal = price * quantity;
                if (product.Discount != 0)
                {
                    subtotal = subtotal * product.Discount / 100;
                }

                existOrderItem.Quantity = quantity;
                existOrderItem.Price = price;
                existOrderItem.SubTotal = subtotal;
                _logger.LogInformation(existOrderItem.Quantity.ToString());
                _logger.LogInformation(productVariantIndex.Quantity.ToString());

                _dbContext.OrderItems.Update(existOrderItem);
                _dbContext.ProductVariants.Update(productVariantIndex);

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return new OrderItemResponse
                {
                    Id = existOrderItem.Id,
                    ColorId = existOrderItem.ColorId,
                    ColorName = color.ColorName,
                    ProductId = existOrderItem.ProductId,
                    ProductName = existOrderItem.Product.ProductName,
                    Dimension = existOrderItem.Dimension,
                    Price = price,
                    SubTotal = subtotal,
                    Quantity = quantity
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
