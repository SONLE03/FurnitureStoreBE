using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.Constants;
using FurnitureStoreBE.DTOs.Request.ProductRequest;
using FurnitureStoreBE.Services.ProductService.ProductService;
using FurnitureStoreBE.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FurnitureStoreBE.Controllers.ProductController
{
    [ApiController]
    [Route(Routes.PRODUCTVARIANT)]
    public class ProductVariantController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductVariantController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpPost("{productId}")]
        public async Task<IActionResult> AddProductVariant(Guid productId, [FromForm] List<ProductVariantRequest> productVariantRequests)
        {
            return new SuccessfulResponse<object>(await _productService.AddProductVariants(productId, productVariantRequests), (int)HttpStatusCode.Created, "Product variants created successfully").GetResponse();
        }
        [HttpPut("{productVariantId}")]
        public async Task<IActionResult> UpdateProductVariant(Guid productVariantId, [FromForm] ProductVariantRequest productVariantRequest)
        {
            return new SuccessfulResponse<object>(await _productService.UpdateProductVariant(productVariantId, productVariantRequest), (int)HttpStatusCode.OK, "Product variant modified successfully").GetResponse();
        }
        [HttpDelete("{productVariantId}")]
        public async Task<IActionResult> DeleteProductVariant(Guid productVariantId)
        {
            await _productService.DeleteProductVariant(productVariantId);
            return new SuccessfulResponse<object>(null, (int)HttpStatusCode.OK, "Product variants deleted successfully").GetResponse();
        }
    }
}
