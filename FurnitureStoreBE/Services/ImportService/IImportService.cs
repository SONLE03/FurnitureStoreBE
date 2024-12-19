using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.Common;
using FurnitureStoreBE.DTOs.Request.ImportRequest;
using FurnitureStoreBE.DTOs.Response.ImportResponse;

namespace FurnitureStoreBE.Services.ImportService
{
    public interface IImportService
    {
        Task<ImportResponse> GetImportById(Guid importId);
        Task<ImportResponse> CreateImport(List<ImportRequest> importRequest);
        Task<PaginatedList<ImportResponse>> GetImports(PageInfo pageInfo);

    }
}
