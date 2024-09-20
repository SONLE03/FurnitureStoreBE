﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using FurnitureStoreBE.Common;
using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.Data;
using FurnitureStoreBE.DTOs.Request.BrandRequest;
using FurnitureStoreBE.DTOs.Response.BrandResponse;
using FurnitureStoreBE.Enums;
using FurnitureStoreBE.Exceptions;
using FurnitureStoreBE.Models;
using FurnitureStoreBE.Services.FileUploadService;
using Microsoft.EntityFrameworkCore;

namespace FurnitureStoreBE.Services.BrandService
{
    public class BrandServiceImp : IBrandService
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly IFileUploadService _fileUploadService;
        private readonly IMapper _mapper;
        public BrandServiceImp(ApplicationDBContext dbContext, IFileUploadService fileUploadService, IMapper mapper)
        {
            _dbContext = dbContext;
            _fileUploadService = fileUploadService;
            _mapper = mapper;
        }
        public async Task<PaginatedList<BrandResponse>> GetAllBrands(PageInfo pageInfo)
        {
            var brandQuery = _dbContext.Brands
                .Where(b => !b.IsDeleted)
                .OrderByDescending(b => b.CreatedDate)
                .ProjectTo<BrandResponse>(_mapper.ConfigurationProvider);
            var count = await _dbContext.Brands.CountAsync();
            return await Task.FromResult(PaginatedList<BrandResponse>.ToPagedList(brandQuery, pageInfo.PageNumber, pageInfo.PageSize));
        }

        public async Task ChangeBrandImage(Guid id, IFormFile formFile)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var brand = await _dbContext.Brands.FirstOrDefaultAsync(b => b.Id == id);
                if (brand == null) throw new ObjectNotFoundException("Brand not found");
                Asset brandImage = new Asset();
                if (brand.AssetId == null)
                {
                    brandImage.Brand = brand;
                }
                else
                {
                    brandImage.Id = (Guid)brand.AssetId;
                    await _fileUploadService.DestroyFileByAssetIdAsync(brandImage.Id);
                }

                var avatarUploadResult = await _fileUploadService.UploadFileAsync(formFile, EUploadFileFolder.Avatar.ToString());
                brandImage.Name = avatarUploadResult.OriginalFilename;
                brandImage.URL = avatarUploadResult.Url.ToString();
                brandImage.CloudinaryId = avatarUploadResult.PublicId;
                brandImage.FolderName = EUploadFileFolder.Avatar.ToString();
                if (brand.AssetId == null)
                {
                    await _dbContext.Assets.AddAsync(brandImage);
                }
                else
                {
                    _dbContext.Assets.Update(brandImage);
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

        public async Task<BrandResponse> CreateBrand(BrandRequest brandRequest, IFormFile file)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var brandImageUploadResult = await _fileUploadService.UploadFileAsync(file, EUploadFileFolder.Brand.ToString());
                var asset = new Asset
                {
                    Name = brandImageUploadResult.OriginalFilename,
                    URL = brandImageUploadResult.Url.ToString(),
                    CloudinaryId = brandImageUploadResult.PublicId,
                    FolderName = EUploadFileFolder.Brand.ToString(),
                };
                var brand = new Brand { BrandName = brandRequest.BrandName, Description = brandRequest.Description, Asset = asset};
                brand.setCommonCreate(UserSession.GetUserId());
                await _dbContext.Brands.AddAsync(brand);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return _mapper.Map<BrandResponse>(brand);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteBrand(Guid id)
        {
            if (!await _dbContext.Brands.AnyAsync(b => b.Id == id)) throw new ObjectNotFoundException("Brand not found");
            var sql = "DELETE FROM Brand WHERE Id = @p0";
            int affectedRows = await _dbContext.Database.ExecuteSqlRawAsync(sql, id);
            if (affectedRows == 0)
            {  
                throw new BusinessException("Brand removal failed");
            }
            sql = "UPDATE Brand SET IsDeleted = @p0 WHERE Id = @p1";
            await _dbContext.Database.ExecuteSqlRawAsync(sql, true, id);
        }

        public async Task<BrandResponse> UpdateBrand(Guid id, BrandRequest brandRequest)
        {
            var brand = await _dbContext.Brands.FirstAsync(b => b.Id == id);
            if (brand == null) throw new ObjectNotFoundException("Brand not found");
            brand.BrandName = brandRequest.BrandName;
            brand.Description = brandRequest.Description;
            brand.setCommonUpdate(UserSession.GetUserId());
            _dbContext.Brands.Update(brand);
            await _dbContext.SaveChangesAsync();
            return _mapper.Map<BrandResponse>(brand);
        }
    }
}