using FurnitureStoreBE.Common;
using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.DTOs.Request.BrandRequest;
using FurnitureStoreBE.DTOs.Response.BrandResponse;

namespace FurnitureStoreBE.Services.BrandService
{
    public interface IBrandService
    {
        Task<PaginatedList<BrandResponse>> GetAllBrands(PageInfo pageInfo);
        Task<BrandResponse> CreateBrand(BrandRequest brandRequest, IFormFile formFile);
        Task<BrandResponse> UpdateBrand(Guid id, BrandRequest brandRequest);
        Task DeleteBrand(Guid id);
        Task ChangeBrandImage(Guid id, IFormFile formFile);
    }
}
