using FurnitureStoreBE.Constants;
using FurnitureStoreBE.DTOs.Request.ProductRequest;
using FurnitureStoreBE.Services.ProductService.FavoriteProductService;
using FurnitureStoreBE.Services.ProductService.ProductService;
using FurnitureStoreBE.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FurnitureStoreBE.Controllers.ProductController
{
    [ApiController]
    [Route(Routes.FAVORITE)]
    public class FavoriteProductController : ControllerBase
    {
        private readonly IFavoriteProductService _favoriteProductService;
        public FavoriteProductController(IFavoriteProductService favoriteProductService)
        {
            _favoriteProductService = favoriteProductService;
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetFavoriteProducts(string userId)
        {
            return new SuccessfulResponse<object>(await _favoriteProductService.GetFavoriteProducts(userId), (int)HttpStatusCode.OK, "Get your favorite products successfully").GetResponse();

        }
        [HttpPost("{userId}/{productId}")]
        public async Task<IActionResult> AddFavoriteProduct(string userId, Guid productId)
        {
            return new SuccessfulResponse<object>(await _favoriteProductService.AddFavoriteProduct(userId, productId), (int)HttpStatusCode.Created, "Your favorite product has been successfully added").GetResponse();
        }
        [HttpDelete("{userId}")]
        public async Task<IActionResult> RemoveAllFavoriteProduct(string userId)
        {
            await _favoriteProductService.RemoveAllFavoriteProduct(userId);
            return new SuccessfulResponse<object>(null, (int)HttpStatusCode.OK, "All favorite products have been successfully deleted\r\n").GetResponse();
        }
        [HttpDelete("{userId}/{productId}")]
        public async Task<IActionResult> RemoveFavoriteProduct(string userId, Guid productId)
        {
            await _favoriteProductService.RemoveFavoriteProduct(userId, productId);
            return new SuccessfulResponse<object>(null, (int)HttpStatusCode.OK, "Favorite products have been successfully deleted\r\n").GetResponse();
        }
    }
}
