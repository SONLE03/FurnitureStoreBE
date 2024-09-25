using AutoMapper;
using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.Common;
using FurnitureStoreBE.Data;
using FurnitureStoreBE.Enums;
using FurnitureStoreBE.Exceptions;
using FurnitureStoreBE.Models;
using FurnitureStoreBE.Services.FileUploadService;
using FurnitureStoreBE.DTOs.Response.ProductResponse;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using FurnitureStoreBE.DTOs.Request.ProductRequest;

namespace FurnitureStoreBE.Services.ProductService.MaterialService
{
    public class MaterialServiceImp : IMaterialService
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly IFileUploadService _fileUploadService;
        private readonly IMapper _mapper;
        public MaterialServiceImp(ApplicationDBContext dbContext, IFileUploadService fileUploadService, IMapper mapper)
        {
            _dbContext = dbContext;
            _fileUploadService = fileUploadService;
            _mapper = mapper;
        }
        public async Task<PaginatedList<MaterialResponse>> GetAllMaterials(PageInfo pageInfo)
        {
            var materialQuery = _dbContext.Materials
                .Where(b => !b.IsDeleted)
                .OrderByDescending(b => b.CreatedDate)
                .ProjectTo<MaterialResponse>(_mapper.ConfigurationProvider);
            var count = await _dbContext.Materials.CountAsync();
            return await Task.FromResult(PaginatedList<MaterialResponse>.ToPagedList(materialQuery, pageInfo.PageNumber, pageInfo.PageSize));
        }

        public async Task ChangeMaterialImage(Guid id, IFormFile formFile)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var material = await _dbContext.Materials.FirstOrDefaultAsync(b => b.Id == id);
                if (material == null) throw new ObjectNotFoundException("Material not found");
                Asset materialImage = new Asset();
                if (material.AssetId == null)
                {
                    materialImage.Material = material;
                }
                else
                {
                    materialImage.Id = (Guid)material.AssetId;
                    await _fileUploadService.DestroyFileByAssetIdAsync(materialImage.Id);
                }

                var materialImageUploadResult = await _fileUploadService.UploadFileAsync(formFile, EUploadFileFolder.Material.ToString());
                materialImage.Name = materialImageUploadResult.OriginalFilename;
                materialImage.URL = materialImageUploadResult.Url.ToString();
                materialImage.CloudinaryId = materialImageUploadResult.PublicId;
                materialImage.FolderName = EUploadFileFolder.Material.ToString();
                if (material.AssetId == null)
                {
                    await _dbContext.Assets.AddAsync(materialImage);
                }
                else
                {
                    _dbContext.Assets.Update(materialImage);
                }
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<MaterialResponse> CreateMaterial(MaterialRequest materialRequest)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                Asset asset = null;
                if(materialRequest.Image != null)
                {
                    var materialImageUploadResult = await _fileUploadService.UploadFileAsync(materialRequest.Image, EUploadFileFolder.Material.ToString());
                    asset = new Asset
                    {
                        Name = materialImageUploadResult.OriginalFilename,
                        URL = materialImageUploadResult.Url.ToString(),
                        CloudinaryId = materialImageUploadResult.PublicId,
                        FolderName = EUploadFileFolder.Material.ToString(),
                    };
                }
                var material = new Material { MaterialName = materialRequest.MaterialName, Description = materialRequest.Description, Asset = asset };
                material.setCommonCreate(UserSession.GetUserId());
                await _dbContext.Materials.AddAsync(material);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return _mapper.Map<MaterialResponse>(material);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteMaterial(Guid id)
        {
            try
            {
                if (!await _dbContext.Materials.AnyAsync(b => b.Id == id)) throw new ObjectNotFoundException("Material not found");
                var sql = "DELETE FROM \"Material\" WHERE \"Id\" = @p0";
                int affectedRows = await _dbContext.Database.ExecuteSqlRawAsync(sql, id);
                if (affectedRows == 0)
                {
                    sql = "UPDATE \"Material\" SET \"IsDeleted\" = @p0 WHERE \"Id\" = @p1";
                    await _dbContext.Database.ExecuteSqlRawAsync(sql, true, id);
                }
            }
            catch
            {
                throw new BusinessException("Material removal failed");
            }
           
        }

        public async Task<MaterialResponse> UpdateMaterial(Guid id, MaterialRequest materialRequest)
        {
            var material = await _dbContext.Materials.FirstAsync(b => b.Id == id);
            if (material == null) throw new ObjectNotFoundException("Material not found");
            material.MaterialName = materialRequest.MaterialName;
            material.Description = materialRequest.Description;
            material.setCommonUpdate(UserSession.GetUserId());
            _dbContext.Materials.Update(material);
            await _dbContext.SaveChangesAsync();
            return _mapper.Map<MaterialResponse>(material);
        }
    }
}
