using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.Common;
using FurnitureStoreBE.DTOs.Response.ProductResponse;
using FurnitureStoreBE.DTOs.Request.ProductRequest;

namespace FurnitureStoreBE.Services.ProductService.CategoryService
{
    public interface ICategoryService
    {
        Task<PaginatedList<CategoryResponse>> GetAllCategories(PageInfo pageInfo);
        Task<CategoryResponse> CreateCategory(CategoryRequest categoryRequest, IFormFile formFile);
        Task<CategoryResponse> UpdateCategory(Guid id, CategoryRequest categoryRequest);
        Task DeleteCategory(Guid id);
        Task ChangeCategoryImage(Guid id, IFormFile formFile);
    }
}
