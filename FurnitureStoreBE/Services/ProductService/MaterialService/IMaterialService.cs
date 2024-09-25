using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.Common;

using FurnitureStoreBE.DTOs.Response.ProductResponse;
using FurnitureStoreBE.DTOs.Request.ProductRequest;

namespace FurnitureStoreBE.Services.ProductService.MaterialService
{
    public interface IMaterialService
    {
        Task<PaginatedList<MaterialResponse>> GetAllMaterials(PageInfo pageInfo);
        Task<MaterialResponse> CreateMaterial(MaterialRequest raterialRequest);
        Task<MaterialResponse> UpdateMaterial(Guid id, MaterialRequest raterialRequest);
        Task DeleteMaterial(Guid id);
        Task ChangeMaterialImage(Guid id, IFormFile formFile);
    }
}
