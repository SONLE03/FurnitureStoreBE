using AutoMapper;
using AutoMapper.QueryableExtensions;
using FurnitureStoreBE.Common;
using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.Data;
using FurnitureStoreBE.DTOs.Request.ImportRequest;
using FurnitureStoreBE.DTOs.Response.BrandResponse;
using FurnitureStoreBE.DTOs.Response.ImportResponse;
using FurnitureStoreBE.Exceptions;
using FurnitureStoreBE.Models;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;

namespace FurnitureStoreBE.Services.ImportService
{
    public class ImportServiceImp : IImportService
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDBContext _dbContext;
        public ImportServiceImp(IMapper mapper, ApplicationDBContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }
        public async Task<ImportResponse> CreateImport(List<ImportRequest> importRequest)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var productVariantIds = importRequest.Select(x => x.ProductVariantId).ToList();
                var productVariants = await _dbContext.ProductVariants
                    .Where(p => productVariantIds.Contains(p.Id))
                    .ToDictionaryAsync(p => p.Id);
                decimal total = 0;
                var importInvoice = new ImportInvoice();
                var importItems = new List<ImportItem>();
                foreach (var item in importRequest)
                {
                    if (!productVariants.TryGetValue(item.ProductVariantId, out var productVariant))
                    {
                        throw new BusinessException("Product variant not found");
                    }

                    var importItem = new ImportItem
                    {
                        ProductVariant = productVariant,
                        ImportInvoiceId = importInvoice.Id,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        Total = item.Total
                    };

                    total += importItem.Total;  // Correct the total calculation
                    importItems.Add(importItem);

                    productVariant.Quantity += item.Quantity;
                }
                importInvoice.Total = total;  // Set total for the invoice
                importInvoice.ImportItem = importItems;
                importInvoice.setCommonCreate(UserSession.GetUserId());
                await _dbContext.ImportInvoices.AddAsync(importInvoice);
                _dbContext.ProductVariants.UpdateRange(productVariants.Values);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return _mapper.Map<ImportResponse>(importInvoice);

            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<ImportResponse> GetImportById(Guid importId)
        {
            if(!await _dbContext.ImportInvoices.AnyAsync(ii => ii.Id  == importId))
            {
                throw new BusinessException("Import invoice not found");
            }
            return _mapper.Map<ImportResponse>(await _dbContext.ImportInvoices.SingleOrDefaultAsync(ii => ii.Id == importId));
        }

        public async Task<PaginatedList<ImportResponse>> GetImports(PageInfo pageInfo)
        {
            var importQuery = _dbContext.ImportInvoices
                .Where(b => !b.IsDeleted)
                .OrderByDescending(b => b.CreatedDate)
                .ProjectTo<ImportResponse>(_mapper.ConfigurationProvider);
            var count = await _dbContext.ImportInvoices.CountAsync();
            return await Task.FromResult(PaginatedList<ImportResponse>.ToPagedList(importQuery, pageInfo.PageNumber, pageInfo.PageSize));
        }
    }
}
