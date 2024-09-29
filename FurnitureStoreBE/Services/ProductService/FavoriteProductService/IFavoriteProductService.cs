using FurnitureStoreBE.DTOs.Response.ProductResponse;

namespace FurnitureStoreBE.Services.ProductService.FavoriteProductService
{
    public interface IFavoriteProductService
    {
        Task<List<ProductResponse>> GetFavoriteProducts(string userId);
        Task<ProductResponse> AddFavoriteProduct(string userId, Guid productId);
        Task RemoveFavoriteProduct(string userId, Guid productId);
        Task RemoveAllFavoriteProduct(string userId);
    }
}
