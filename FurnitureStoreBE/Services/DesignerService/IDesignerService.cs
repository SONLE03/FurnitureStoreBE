﻿using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.Common;
using FurnitureStoreBE.DTOs.Request.ProductRequest;
using FurnitureStoreBE.DTOs.Response.ProductResponse;

namespace FurnitureStoreBE.Services.DesignerService
{
    public interface IDesignerService
    {
        Task<PaginatedList<DesignerResponse>> GetAllDesigners(PageInfo pageInfo);
        Task<DesignerResponse> CreateDesigner(DesignerRequest designerRequest, IFormFile formFile);
        Task<DesignerResponse> UpdateDesigner(Guid id, DesignerRequest designerRequest);
        Task DeleteDesigner(Guid id);
        Task ChangeDesignerImage(Guid id, IFormFile formFile);
    }
}
