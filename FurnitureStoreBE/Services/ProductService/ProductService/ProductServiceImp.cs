using AutoMapper;
using AutoMapper.QueryableExtensions;
using FurnitureStoreBE.Common;
using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.Data;
using FurnitureStoreBE.DTOs.Request.ProductRequest;
using FurnitureStoreBE.DTOs.Response.ProductResponse;
using FurnitureStoreBE.Enums;
using FurnitureStoreBE.Exceptions;
using FurnitureStoreBE.Models;
using FurnitureStoreBE.Services.FileUploadService;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace FurnitureStoreBE.Services.ProductService.ProductService
{
    public class ProductServiceImp : IProductService
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly IFileUploadService _fileUploadService;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductServiceImp> _log;
        public ProductServiceImp(ApplicationDBContext dbContext, IFileUploadService fileUploadService, IMapper mapper, ILogger<ProductServiceImp> log)
        {
            _dbContext = dbContext;
            _fileUploadService = fileUploadService;
            _mapper = mapper;
            _log = log;
        }
        public IQueryable<ProductResponse> GetProductByLinqQuery(IQueryable<Product> query)
        {
            var productList = query
                .Select(product => new ProductResponse
                {
                    Id = product.Id,
                    ProductName = product.ProductName,
                    ImageSource = product.Asset != null ? product.Asset.URL : null,
                    Unit = product.Unit,
                    Description = product.Description,
                    BrandName = product.Brand != null ? product.Brand.BrandName : null,
                    CategoryName = product.Category != null ? product.Category.CategoryName : null,
                    DisplayPrice = $"{product.MinPrice} - {product.MaxPrice}",
                    Discount = product.Discount,
                    Materials = product.Materials.Select(m => m.MaterialName).ToList(),
                    Designers = product.Designers.Select(d => d.DesignerName).ToList(),
                    ProductVariants = product.ProductVariants
                        .Where(pv => !pv.IsDeleted)
                        .Select(v => new ProductVariantResponse
                        {
                            Id = v.Id,
                            ColorId = v.ColorId,
                            ColorName = v.Color != null ? v.Color.ColorName : null,
                            DisplayDimension = v.DisplayDimension,
                            Quantity = v.Quantity,
                            Price = v.Price,
                            ImageSource = v.Assets.Select(a => a.URL).ToList()
                        }).ToList()
                });

            return productList;
        }

        public async Task<List<ProductResponse>> GetProducts(IQueryable<Product> query)
        {
            var productList = await GetProductByLinqQuery(query).ToListAsync();
            return productList;
        }

        public async Task<ProductResponse> GetProductById(Guid productId)
        {
            if(!await _dbContext.Products.AnyAsync(p => p.Id == productId))
            {
                throw new ObjectNotFoundException("Product not found");
            }
            var productQuery = _dbContext.Products.Where(p => p.Id == productId);
            var product = await GetProductByLinqQuery(productQuery).SingleOrDefaultAsync();
            return product;
        }

        public async Task<PaginatedList<ProductResponse>> GetAllProduct(PageInfo pageInfo, ProductSearchRequest productSearchRequest)
        {
            var productQuery = _dbContext.Products
                .Where(b => !b.IsDeleted)
                .Where(product => product.ProductVariants.Any(pv => !pv.IsDeleted));
            if(productSearchRequest.BrandIds != null)
            {
                if (await _dbContext.Brands.AnyAsync(b => productSearchRequest.BrandIds.Contains(b.Id)))
                {
                    productQuery = productQuery.Where(b => productSearchRequest.BrandIds.Contains((Guid)b.BrandId));
                }
            }
            if(productSearchRequest.CategoryIds != null)
            {
                if (await _dbContext.Brands.AnyAsync(c => productSearchRequest.CategoryIds.Contains(c.Id)))
                {
                    productQuery = productQuery.Where(c => productSearchRequest.CategoryIds.Contains((Guid)c.CategoryId));
                }
            }
            if (productSearchRequest.FromPrice.HasValue && productSearchRequest.ToPrice.HasValue)
            {
                productQuery = productQuery.Where(price => price.MinPrice >= productSearchRequest.FromPrice.Value
                                                            && price.MaxPrice <= productSearchRequest.ToPrice.Value);
            }
            var productResponseQuery = productQuery
                .OrderByDescending(b => b.CreatedDate)
                .Select(product => new ProductResponse
                {
                    Id = product.Id,
                    ProductName = product.ProductName,
                    ImageSource = product.Asset != null ? product.Asset.URL : null, // Ternary operator instead of null-conditional
                    Unit = product.Unit,
                    Description = product.Description,
                    BrandName = product.Brand != null ? product.Brand.BrandName : null, // Ternary operator for Brand
                    CategoryName = product.Category != null ? product.Category.CategoryName : null, // Ternary operator for Category
                    DisplayPrice = $"{product.MinPrice} - {product.MaxPrice}",
                    Discount = product.Discount,
                    Materials = product.Materials.Select(m => m.MaterialName).ToList(),
                    Designers = product.Designers.Select(d => d.DesignerName).ToList(),
                    ProductVariants = product.ProductVariants
                        .Where(pv => !pv.IsDeleted) // Filter to get only non-deleted variants
                        .Select(v => new ProductVariantResponse
                        {
                            Id = v.Id,
                            ColorId = v.ColorId,
                            ColorName = v.Color != null ? v.Color.ColorName : null, // Ternary operator for Color
                            DisplayDimension = v.DisplayDimension,
                            Quantity = v.Quantity,
                            Price = v.Price,
                            ImageSource = v.Assets.Select(a => a.URL).ToList() // Get URLs of assets
                        }).ToList()
                });
            var count = await _dbContext.Products.Where(b => !b.IsDeleted).CountAsync();
            return await Task.FromResult(PaginatedList<ProductResponse>.ToPagedList(productResponseQuery, pageInfo.PageNumber, pageInfo.PageSize));
        }
        public async Task<ProductResponse> CreateProduct(ProductRequest productRequest)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // Tách các truy vấn AnyAsync ra, đảm bảo không có tác vụ nào chạy đồng thời trên cùng một DbContext
                var brand = await _dbContext.Brands
                    .Where(a => a.Id == productRequest.BrandId)
                    .FirstOrDefaultAsync();
                if (brand == null)
                {
                    throw new ObjectNotFoundException("Brand not found");
                }

                var category = await _dbContext.Categories
                    .Where(a => a.Id == productRequest.CategoryId)
                    .FirstOrDefaultAsync();

                if (category == null)
                {
                    throw new ObjectNotFoundException("Category not found");
                }

                var variants = productRequest.ProductVariants;

                if (variants == null || !variants.Any())
                {
                    throw new ObjectNotFoundException("Variable product must have at least one attribute");
                }
                
                var minPrice = variants.Min(p => p.Price);
                var maxPrice = variants.Max(p => p.Price);

                // Các truy vấn khác như kiểm tra Color, Designer, Materials
                var colorIds = variants.Select(v => v.ColorId).ToList();
                var colors = await _dbContext.Colors
                    .Where(c => colorIds.Contains(c.Id))
                    .ToListAsync();

                var designers = await _dbContext.Designer
                    .Where(d => productRequest.DesignersId
                    .Contains(d.Id))
                    .ToListAsync();
                var materials = await _dbContext.Materials
                    .Where(m => productRequest.MaterialsId
                    .Contains(m.Id))
                    .ToListAsync();

                var uniqueVariants = new HashSet<string>();

                // Tải lên ảnh của từng biến thể sản phẩm
                var productVariants = new List<ProductVariant>();
                foreach (var item in variants)
                {
                    var color = colors.FirstOrDefault(c => c.Id == item.ColorId);

                    if (color == null)
                    {
                        // If the color doesn't exist, throw an exception
                        throw new ObjectNotFoundException($"Color with ID {item.ColorId} not found");
                    }
                    var variantKey = $"{item.ColorId}-{item.Height}-{item.Length}-{item.Width}";

                    // Check if this combination already exists in the HashSet
                    if (!uniqueVariants.Add(variantKey))
                    {
                        throw new Exception($"Duplicate variant found with combination: ColorId={item.ColorId}, Height={item.Height}, Weight={item.Length}, Width={item.Width}");
                    }

                    var productVariantImagesUploadResult = await _fileUploadService.UploadFilesAsync(item.Images, EUploadFileFolder.Product.ToString());
                    var assets = productVariantImagesUploadResult.Select(img => new Asset
                    {
                        Name = img.OriginalFilename,
                        URL = img.Url.ToString(),
                        CloudinaryId = img.PublicId,
                        FolderName = EUploadFileFolder.Product.ToString()
                    }).ToList();

                    productVariants.Add(new ProductVariant
                    {
                        Color = color,
                        Length = item.Length,
                        Width = item.Width,
                        Height = item.Height,
                        DisplayDimension = $"{item.Length} x {item.Width} x {item.Height}",
                        Quantity = item.Quantity,
                        Price = item.Price,
                        Assets = assets
                    });
                }

                var productImageUploadResult = await _fileUploadService.UploadFileAsync(productRequest.Thumbnail, EUploadFileFolder.Product.ToString());
                var asset = new Asset
                {
                    Name = productImageUploadResult.OriginalFilename,
                    URL = productImageUploadResult.Url.ToString(),
                    CloudinaryId = productImageUploadResult.PublicId,
                    FolderName = EUploadFileFolder.Product.ToString()
                };
                decimal discount = productRequest.Discount.HasValue? productRequest.Discount.Value : 0;
                var product = new Product
                {
                    ProductName = productRequest.ProductName,
                    Brand = brand,
                    Category = category,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice,
                    Discount = discount,
                    ProductVariants = productVariants,
                    Description = productRequest.Description,
                    Asset = asset,
                    Unit = productRequest.Unit,
                    Designers = designers,
                    Materials = materials,
                };

                product.setCommonCreate(UserSession.GetUserId());
                await _dbContext.Products.AddAsync(product);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return _mapper.Map<ProductResponse>(product);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        public async Task DeleteProduct(Guid productId)
        {
            try
            {
                if (!await _dbContext.Products.AnyAsync(b => b.Id == productId)) throw new ObjectNotFoundException("Product not found");
                //var sqlDelete = "DELETE FROM \"Product\" WHERE \"Id\" = @p0";
                //int affectedRows = await _dbContext.Database.ExecuteSqlRawAsync(sqlDelete, productId);
                //if (affectedRows == 0)
                //{
                    var sqlUpdate = "UPDATE \"Product\" SET \"IsDeleted\" = @p0 WHERE \"Id\" = @p1"; // Sử dụng dấu ngoặc kép
                    await _dbContext.Database.ExecuteSqlRawAsync(sqlUpdate, true, productId);
                //}
            }
            catch
            {
                throw new BusinessException("Product removal failed");

            }
        }
        public async Task<ProductResponse> UpdateProduct(Guid productId, ProductRequest productRequest)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                if (!await _dbContext.Products.AnyAsync(p => p.Id == productId))
                {
                    throw new ObjectNotFoundException("Product not found");
                }
                var product = await _dbContext.Products
                    .Include(a => a.Asset)
                    .Include(b => b.Brand)
                    .Include(c => c.Category)
                    .Include(d => d.Designers)
                    .Include(m => m.Materials)
                    .SingleOrDefaultAsync(b => b.Id == productId);

                if (productRequest.BrandId != product.BrandId)
                {
                    var brand = await _dbContext.Brands.SingleOrDefaultAsync(b => b.Id == productRequest.BrandId);
                    if (brand == null)
                    {
                        throw new ObjectNotFoundException("Brand not found");
                    }
                    product.Brand = brand;
                }
                if (productRequest.CategoryId != product.CategoryId)
                {
                    var category = await _dbContext.Categories.SingleOrDefaultAsync(b => b.Id == productRequest.CategoryId);
                    if (category == null)
                    {
                        throw new ObjectNotFoundException("Category not found");
                    }
                    product.Category = category;
                }
                var designers = await _dbContext.Designer
                        .Where(d => productRequest.DesignersId
                        .Contains(d.Id))
                        .ToListAsync();
                var materials = await _dbContext.Materials
                    .Where(m => productRequest.MaterialsId
                    .Contains(m.Id))
                    .ToListAsync();
                decimal discount = productRequest.Discount.HasValue ? productRequest.Discount.Value : 0;
                product.Discount = discount;
                product.Designers = designers;
                product.Materials = materials;
                product.Description = productRequest.Description;
                product.setCommonUpdate(UserSession.GetUserId());
                _dbContext.Products.Update(product);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return _mapper.Map<ProductResponse>(product);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<ProductResponse> AddProductVariants(Guid productId, List<ProductVariantRequest> productVariantsRequest)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                if(!await _dbContext.Products.AnyAsync(p => p.Id == productId)){
                    throw new ObjectNotFoundException("Product not found");
                }
                var product = await _dbContext.Products
                    .Include(a => a.Asset)
                    .Include(b => b.Brand)
                    .Include(c => c.Category)
                    .Include(d => d.Designers)
                    .Include(m => m.Materials)
                    .SingleOrDefaultAsync(p => p.Id == productId);
               
                var variants = productVariantsRequest;

                if (variants == null || !variants.Any())
                {
                    throw new ObjectNotFoundException("Variable product must have at least one attribute");
                }

                var minPrice = Math.Min(product.MinPrice, variants.Min(p => p.Price));
                var maxPrice = Math.Max(product.MaxPrice, variants.Max(p => p.Price));

                // Các truy vấn khác như kiểm tra Color, Designer, Materials
                var colorIds = variants.Select(v => v.ColorId).ToList();
                var colors = await _dbContext.Colors
                    .Where(c => colorIds.Contains(c.Id))
                    .ToListAsync();
                var uniqueVariants = new HashSet<string>();
                var productVariants = new List<ProductVariant>();
                foreach (var item in variants)
                {
                    var color = colors.FirstOrDefault(c => c.Id == item.ColorId);

                    if (color == null)
                    {
                        // If the color doesn't exist, throw an exception
                        throw new ObjectNotFoundException($"Color with ID {item.ColorId} not found");
                    }
                    var variantKey = $"{item.ColorId}-{item.Height}-{item.Length}-{item.Width}";

                    // Check if this combination already exists in the HashSet
                    if (!uniqueVariants.Add(variantKey))
                    {
                        throw new BusinessException($"Duplicate variant found with combination: ColorId={item.ColorId}, Height={item.Height}, Weight={item.Length}, Width={item.Width}");
                    }

                    var productVariantImagesUploadResult = await _fileUploadService.UploadFilesAsync(item.Images, EUploadFileFolder.Product.ToString());
                    var assets = productVariantImagesUploadResult.Select(img => new Asset
                    {
                        Name = img.OriginalFilename,
                        URL = img.Url.ToString(),
                        CloudinaryId = img.PublicId,
                        FolderName = EUploadFileFolder.Product.ToString()
                    }).ToList();

                    productVariants.Add(new ProductVariant
                    {
                        ProductId = product.Id,
                        Color = color,
                        Length = item.Length,
                        Width = item.Width,
                        Height = item.Height,
                        DisplayDimension = $"{item.Length} x {item.Width} x {item.Height}",
                        Quantity = item.Quantity,
                        Price = item.Price,
                        Assets = assets
                    });
                }
                product.MinPrice = minPrice;
                product.MaxPrice = maxPrice;
                _dbContext.Products.Update(product);
                await _dbContext.ProductVariants.AddRangeAsync(productVariants);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ProductResponse
                {
                    Id = product.Id,
                    ProductName = product.ProductName,
                    Unit = product.Unit,
                    Description = product.Description,
                    ImageSource = product.Asset.URL,
                    BrandName = product.Brand.BrandName,
                    CategoryName = product.Category.CategoryName,
                    Materials = product.Materials.Select(m => m.MaterialName).ToList(),
                    Designers = product.Designers.Select(m => m.DesignerName).ToList(),
                    ProductVariants = _mapper.Map<List<ProductVariantResponse>>(product.ProductVariants)
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<ProductResponse> UpdateProductVariant(Guid productVariantId, ProductVariantRequest productVariantRequest)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {

                var productVariant = await _dbContext.ProductVariants
                    .Include(c => c.Color)
                    .SingleOrDefaultAsync(p => p.Id == productVariantId);
                if (productVariant == null)
                {
                    throw new ObjectNotFoundException("Product variant not found");
                }
                var dimension = $"{productVariantRequest.Length} x {productVariantRequest.Width} x {productVariantRequest.Height}";

                if (!await _dbContext.ProductVariants.AnyAsync(pv => pv.ProductId == productVariant.ProductId && pv.DisplayDimension.Equals(dimension)))
                {
                    throw new BusinessException("Duplicate variant found");
                }

                var productVariantPrice = productVariantRequest.Price;
                var minPrice = Math.Min(productVariant.Product.MinPrice, productVariantPrice);
                var maxPrice = Math.Max(productVariant.Product.MaxPrice, productVariantPrice);
                var colorId = productVariantRequest.ColorId;
                if(colorId != productVariant.ColorId)
                {
                    if (!await _dbContext.Colors.AnyAsync(c => c.Id == colorId))
                    {
                        throw new ObjectNotFoundException("Color not found");
                    }
                    productVariant.ColorId = colorId;
                }
                productVariant.Length = productVariantRequest.Length;
                productVariant.Width = productVariantRequest.Width;
                productVariant.Height = productVariantRequest.Height;
                productVariant.DisplayDimension = dimension;
                productVariant.Quantity = productVariantRequest.Quantity;
                productVariant.Price = productVariantRequest.Price;

                productVariant.Product.MinPrice = minPrice;
                productVariant.Product.MaxPrice = maxPrice;
                _dbContext.ProductVariants.Update(productVariant);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ProductResponse
                {
                    Id = productVariant.Product.Id,
                    ProductName = productVariant.Product.ProductName,
                    Unit = productVariant.Product.Unit,
                    Description = productVariant.Product.Description,
                    ImageSource = productVariant.Product.Asset.URL,
                    BrandName = productVariant.Product.Brand.BrandName,
                    CategoryName = productVariant.Product.Category.CategoryName,
                    Materials = productVariant.Product.Materials.Select(m => m.MaterialName).ToList(),
                    Designers = productVariant.Product.Designers.Select(m => m.DesignerName).ToList(),
                    ProductVariants = _mapper.Map<List<ProductVariantResponse>>(productVariant.Product.ProductVariants)
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task DeleteProductVariant(Guid productVariantId)
        {

            try
            {
                if (!await _dbContext.Products.AnyAsync(b => b.Id == productVariantId)) throw new ObjectNotFoundException("Product variant not found");
                var sqlDelete = "DELETE FROM \"ProductVariant\" WHERE \"Id\" = @p0";
                int affectedRows = await _dbContext.Database.ExecuteSqlRawAsync(sqlDelete, productVariantId);
                if (affectedRows == 0)
                {
                    var sqlUpdate = "UPDATE \"ProductVariant\" SET \"IsDeleted\" = @p0 WHERE \"Id\" = @p1"; // Sử dụng dấu ngoặc kép
                    await _dbContext.Database.ExecuteSqlRawAsync(sqlUpdate, true, productVariantId);
                }
            }
            catch
            {
                throw new BusinessException("Product variant removal failed");

            }
        }
        public async Task<(Guid imageId, string imageUrl)> ChangeThumbnail(Guid productId, IFormFile file)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var product = await _dbContext.Products.Where(p => p.Id == productId).SingleOrDefaultAsync();
                if (product == null)
                {
                    throw new ObjectNotFoundException("Product not found.");
                }
                Asset productImage = new Asset();
                if (product.AssetId != null)
                {
                    productImage.Id = (Guid)product.AssetId;
                    await _fileUploadService.DestroyFileByAssetIdAsync((Guid)product.AssetId);
                }
                else
                {
                    productImage.Product = product;
                }

                var productThumbnailUploadResult = await _fileUploadService.UploadFileAsync(file, EUploadFileFolder.Product.ToString());
                productImage.Name = productThumbnailUploadResult.OriginalFilename;
                productImage.URL = productThumbnailUploadResult.Url.ToString();
                productImage.CloudinaryId = productThumbnailUploadResult.PublicId;
                productImage.FolderName = EUploadFileFolder.Product.ToString();
                if (product.AssetId == null)
                {
                    await _dbContext.Assets.AddAsync(productImage);
                }
                else
                {
                    _dbContext.Assets.Update(productImage);
                }
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return (productImage.Id, productImage.URL);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<List<(Guid imageId, string imageUrl)>> ChangeProductVariantImages(Guid productVariantId, List<IFormFile> files)
        {
            //await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            //try
            //{
            //    var productVariant = await _dbContext.ProductVariants.Where(p => p.Id == productVariantId).SingleOrDefaultAsync();
            //    if (productVariant == null)
            //    {
            //        throw new ObjectNotFoundException("Product variant not found.");
            //    }
            //    Asset productVariantImage = new Asset();
            //    if (productVariant.AssetId != null)
            //    {
            //        productVariantImage.Id = (Guid)productVariant.AssetId;
            //        await _fileUploadService.DestroyFileByAssetIdAsync((Guid)product.AssetId);
            //    }
            //    else
            //    {
            //        productImage.Product = product;
            //    }

            //var productThumbnailUploadResult = await _fileUploadService.UploadFileAsync(file, EUploadFileFolder.Product.ToString());
            //productImage.Name = productThumbnailUploadResult.OriginalFilename;
            //productImage.URL = productThumbnailUploadResult.Url.ToString();
            //productImage.CloudinaryId = productThumbnailUploadResult.PublicId;
            //productImage.FolderName = EUploadFileFolder.Product.ToString();
            //if (product.AssetId == null)
            //{
            //    await _dbContext.Assets.AddAsync(productImage);
            //}
            //else
            //{
            //    _dbContext.Assets.Update(productImage);
            //}
            //await _dbContext.SaveChangesAsync();
            //await transaction.CommitAsync();
            //return (productImage.Id, productImage.URL);
            //    return null;
            //}
            //catch
            //{
            //    await transaction.RollbackAsync();
            //    throw;
            //}
            return null;
        }
        public async Task UpdateDiscountValueForProducts(List<Guid> productIds, decimal discountValue)
        {
            if(discountValue < 0)
            {
                throw new BusinessException("Discount value must be greater than 0");
            }
            if (!await _dbContext.Products.AnyAsync(p => productIds.Contains(p.Id)))
            {
                throw new ObjectNotFoundException("No matching products found for the provided IDs.");
            }
            var productsToUpdate = await _dbContext.Products
                .Where(product => productIds.Contains(product.Id))
                .ToListAsync();
            foreach (var product in productsToUpdate)
            {
                product.Discount = discountValue;
            }
            _dbContext.Products.UpdateRange(productsToUpdate);
            await _dbContext.SaveChangesAsync();
        }
    }
}
