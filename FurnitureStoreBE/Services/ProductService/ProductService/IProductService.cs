using FurnitureStoreBE.Common;
using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.DTOs.Request.ProductRequest;
using FurnitureStoreBE.DTOs.Response.ProductResponse;
using FurnitureStoreBE.Models;

namespace FurnitureStoreBE.Services.ProductService.ProductService
{
    public interface IProductService
    {
        Task<List<ProductResponse>> GetProducts(IQueryable<Product> query);
        // product
        Task<ProductResponse> GetProductById(Guid productId);
        Task<PaginatedList<ProductResponse>> GetAllProduct(PageInfo pageInfo, ProductSearchRequest productSearchRequest);
        Task<ProductResponse> CreateProduct(ProductRequest productRequest);
        Task<ProductResponse> UpdateProduct(Guid productId, ProductRequest productRequest);
        Task DeleteProduct(Guid productId);
        Task<(Guid imageId, string imageUrl)> ChangeThumbnail(Guid productId, IFormFile file);

        //product variant
        Task<ProductResponse> AddProductVariants(Guid productId, List<ProductVariantRequest> productVariantsRequest);
        Task<ProductResponse> UpdateProductVariant(Guid productVariantId, ProductVariantRequest productVariantRequest);
        Task DeleteProductVariant(Guid productVariantId);
        // Image
        Task<List<(Guid imageId, string imageUrl)>> ChangeProductVariantImages(Guid productVariantId, List<IFormFile> files);

        // Discount
        Task UpdateDiscountValueForProducts(List<Guid> productIds, decimal discountValue);
    }
}
