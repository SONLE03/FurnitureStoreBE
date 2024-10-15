using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.Common;
using FurnitureStoreBE.DTOs.Response.ProductResponse;
using FurnitureStoreBE.DTOs.Request.ProductRequest;
using FurnitureStoreBE.Models;

namespace FurnitureStoreBE.Services.ProductService.CategoryService
{
    public interface ICategoryService
    {
        Task<PaginatedList<CategoryResponse>> GetAllCategories(PageInfo pageInfo);
        Task<CategoryResponse> CreateCategory(CategoryRequest categoryRequest);
        Task<CategoryResponse> UpdateCategory(Guid id, CategoryRequest categoryRequest);
        Task DeleteCategory(Guid id);
        Task ChangeCategoryImage(Guid id, IFormFile formFile);
    }
}
