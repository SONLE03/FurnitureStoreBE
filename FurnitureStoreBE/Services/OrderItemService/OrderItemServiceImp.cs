using AutoMapper;
using FurnitureStoreBE.Data;
using FurnitureStoreBE.DTOs.Request.OrderRequest;
using FurnitureStoreBE.DTOs.Response.OrderResponse;
using FurnitureStoreBE.Exceptions;
using FurnitureStoreBE.Models;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;


namespace FurnitureStoreBE.Services.CartService
{
    public class OrderItemServiceImp : IOrderItemService
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly ILogger<OrderItemServiceImp> _logger;
        private readonly IMapper _mapper;
        public OrderItemServiceImp(ApplicationDBContext dbContext, ILogger<OrderItemServiceImp> logger, IMapper mapper)
        {
            _dbContext = dbContext;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<List<OrderItemResponse>> GetCartItemByUser(string userId)
        {
            if (!await _dbContext.Users.AnyAsync(u => u.Id == userId))
            {
                throw new ObjectNotFoundException("User not found");
            } 
            var listCartItem = await _dbContext.OrderItems
                .Include(ci => ci.Product)
                .Include(ci => ci.Color)
                .Where(ci => ci.UserId.Equals(userId) && ci.OrderId == null)
                //.Select(i => new OrderItemResponse
                //{
                //    Id = i.Id,
                //    ColorId = i.ColorId,
                //    ColorName = i.Color.ColorName,
                //    ProductId = i.ProductId,
                //    ProductName = i.Product.ProductName,                  
                //    Dimension = i.Dimension,
                //    Quantity = i.Quantity,
                //    Price = i.Price,
                //    SubTotal = i.SubTotal
                //})
                .ToListAsync();
            return _mapper.Map<List<OrderItemResponse>>(listCartItem);
        }
        public async Task<OrderItemResponse> AddCartItem(OrderItemRequest orderItemRequest,string userId)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var cart = await _dbContext.Carts.Where(c => c.UserId.Equals(userId)).SingleOrDefaultAsync();
                if (cart == null)
                {
                    throw new ObjectNotFoundException("User not found");
                }
                var colorId = orderItemRequest.ColorId;
                var dimension = orderItemRequest.Dimension;
                var productId = orderItemRequest.ProductId;
                var quantity = orderItemRequest.Quantity;

                var existOrderItem = await _dbContext.OrderItems
                    .Where(ot => ot.UserId == userId
                            && ot.ColorId == colorId
                            && ot.Dimension.Equals(dimension)
                            && ot.ProductId == productId)
                    .SingleOrDefaultAsync();
                if (existOrderItem != null)
                {
                    transaction.Commit();

                    return await UpdateCartItemQuantity(existOrderItem.Id, quantity);
                }

                var productVariantIndex = await _dbContext.ProductVariants
                    .Include(p => p.Product)
                    .Include(c => c.Color)
                    .Where(pv => pv.ProductId == productId && pv.ColorId == colorId && pv.DisplayDimension.Equals(dimension))
                    .SingleOrDefaultAsync();
                if (productVariantIndex == null)
                {
                    throw new ObjectNotFoundException("Product variant not found");
                }
                if (productVariantIndex.Quantity < quantity)
                {
                    throw new BusinessException("Product variant not enough");
                }

                productVariantIndex.Quantity -= quantity;
                var discountValue = productVariantIndex.Product.Discount;
                var price = productVariantIndex.Price;
                var subtotal = price * quantity;

                if(discountValue != 0)
                {
                    subtotal = subtotal * (1 - discountValue / 100);
                }
                var orderItem = new OrderItem
                {
                    ProductId = productId,
                    ColorId = colorId,
                    Dimension = dimension,
                    Quantity = quantity,
                    Price = productVariantIndex.Price,
                    SubTotal = subtotal,
                    UserId = userId,
                    Cart = cart
                };
                await _dbContext.OrderItems.AddAsync(orderItem);
                _dbContext.ProductVariants.Update(productVariantIndex);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return new OrderItemResponse
                {
                    Id = orderItem.Id,
                    ColorId = colorId,
                    ColorName = productVariantIndex.Color.ColorName,
                    ProductId = productId,
                    ProductName = productVariantIndex.Product.ProductName,
                    Dimension = dimension,
                    Price = price,
                    SubTotal = subtotal,
                    Quantity = quantity,
                };
            }
            catch 
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

       

        public async Task RemoveCartItem(Guid orderItemId)
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

        public async Task<OrderItemResponse> UpdateCartItemQuantity(Guid orderItemId, long quantity)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
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

                var productVariantIndex = await _dbContext.ProductVariants
                    .Include(c => c.Color)
                    .Include(p => p.Product)
                    .Where(pv => pv.ProductId == existOrderItem.ProductId && pv.ColorId == existOrderItem.ColorId && pv.DisplayDimension.Equals(existOrderItem.Dimension))
                    .SingleOrDefaultAsync();
                if (productVariantIndex == null)
                {
                    throw new ObjectNotFoundException("Product variant not found");
                }

                productVariantIndex.Quantity += existOrderItem.Quantity;

                if (productVariantIndex.Quantity < quantity)
                {
                    throw new BusinessException("Product variant not enough");
                }

                productVariantIndex.Quantity -= quantity;
                var discountValue = productVariantIndex.Product.Discount;
                var price = productVariantIndex.Price;
                var subtotal = price * quantity;
                if (discountValue != 0)
                {
                    subtotal = subtotal * (1 - discountValue / 100);
                }

                existOrderItem.Quantity = quantity;
                existOrderItem.Price = price;
                existOrderItem.SubTotal = subtotal;

                _dbContext.OrderItems.Update(existOrderItem);
                _dbContext.ProductVariants.Update(productVariantIndex);

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return new OrderItemResponse
                {
                    Id = existOrderItem.Id,
                    ColorId = existOrderItem.ColorId,
                    ColorName = productVariantIndex.Color.ColorName,
                    ProductId = existOrderItem.ProductId,
                    ProductName = productVariantIndex.Product.ProductName,
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
