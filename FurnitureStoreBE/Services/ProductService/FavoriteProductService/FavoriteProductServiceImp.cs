using AutoMapper;
using FurnitureStoreBE.Data;
using FurnitureStoreBE.DTOs.Response.ProductResponse;
using FurnitureStoreBE.Exceptions;
using FurnitureStoreBE.Models;
using FurnitureStoreBE.Services.ProductService.ProductService;
using Microsoft.EntityFrameworkCore;

namespace FurnitureStoreBE.Services.ProductService.FavoriteProductService
{
    public class FavoriteProductServiceImp : IFavoriteProductService
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        public FavoriteProductServiceImp(ApplicationDBContext dbContext, IMapper mapper, IProductService productService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _productService = productService;
        }
        public async Task<List<ProductResponse>> GetFavoriteProducts(string userId)
        {
            if(!await _dbContext.Users.AnyAsync(u => u.Id == userId))
            {
                throw new ObjectNotFoundException("User not found");
            }
            var favoriteProducts = await _dbContext.Favorites
                .Where(f => f.UserId == userId)
                .Select(p => p.ProductId)
                .ToListAsync();
            var productsQuery = _dbContext.Products
                .Where(p => favoriteProducts.Contains(p.Id));
            var productList = await _productService.GetProducts(productsQuery);
            return productList;
        }
        public async Task<ProductResponse> AddFavoriteProduct(string userId, Guid productId)
        {
            if (await _dbContext.Favorites.AnyAsync(u => u.UserId == userId && u.ProductId == productId))
            {
                var product = await _productService.GetProductById(productId);
                return product;
            }
            if (!await _dbContext.Users.AnyAsync(u => u.Id == userId))
            {
                throw new ObjectNotFoundException("User not found");
            }
            var favorite = new Favorite
            {
                UserId = userId,
                ProductId = productId
            };
            await _dbContext.Favorites.AddAsync(favorite);
            await _dbContext.SaveChangesAsync();
            return await _productService.GetProductById(productId);
        }



        public async Task RemoveAllFavoriteProduct(string userId)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                if (!await _dbContext.Favorites.AnyAsync(u => u.UserId == userId))
                {
                    throw new ObjectNotFoundException("Favorite product not found");
                }
                var sqlDelete = "DELETE FROM \"Favorite\" WHERE \"UserId\" = @p0";
                int affectedRows = await _dbContext.Database.ExecuteSqlRawAsync(sqlDelete, userId);
                if (affectedRows == 0)
                {
                    throw new BusinessException("Failed to remove all favorite products");
                }
                await transaction.CommitAsync();

            }
            catch
            {
                await transaction.RollbackAsync();
                throw new BusinessException("Failed to remove all favorite products");
            }
        }

        public async Task RemoveFavoriteProduct(string userId, Guid productId)
        {
            try
            {
                if (!await _dbContext.Favorites.AnyAsync(f => f.UserId == userId && f.ProductId == productId))
                {
                    throw new ObjectNotFoundException("Favorite product not found");
                }
                var sqlDelete = "DELETE FROM \"Favorite\" WHERE \"UserId\" = @p0 AND \"ProductId\" = @p1";
                int affectedRows = await _dbContext.Database.ExecuteSqlRawAsync(sqlDelete, userId, productId);
                if (affectedRows == 0)
                {
                    throw new BusinessException("Failed to remove favorite products");
                }
            }
            catch
            {
                throw new BusinessException("Failed to remove favorite products");

            }
        }
    }
}
