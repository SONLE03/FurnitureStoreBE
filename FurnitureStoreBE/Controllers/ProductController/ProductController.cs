﻿using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.Constants;
using FurnitureStoreBE.DTOs.Request.ProductRequest;
using FurnitureStoreBE.Services.ProductService.ProductService;
using FurnitureStoreBE.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FurnitureStoreBE.Controllers.ProductController
{
    [ApiController]
    [Route(Routes.PRODUCT)]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpGet()]
        public async Task<IActionResult> GetProducts([FromQuery] PageInfo pageInfo, [FromQuery] ProductSearchRequest productSearchRequest)
        {
            return new SuccessfulResponse<object>(await _productService.GetAllProduct(pageInfo, productSearchRequest), (int)HttpStatusCode.OK, "Product created successfully").GetResponse();
        }
        [HttpPost()]
        public async Task<IActionResult> CreateProduct([FromForm] ProductRequest productRequest)
        {
            return new SuccessfulResponse<object>(await _productService.CreateProduct(productRequest), (int)HttpStatusCode.Created, "Product created successfully").GetResponse();
        }
        [HttpPost("productVariant/{productId}")]
        public async Task<IActionResult> AddProductVariant(Guid productId, [FromForm] List<ProductVariantRequest> productVariantRequests)
        {
            return new SuccessfulResponse<object>(await _productService.AddProductVariants(productId, productVariantRequests), (int)HttpStatusCode.Created, "Product variants created successfully").GetResponse();
        }
        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateProduct(Guid productId, [FromForm] ProductRequest productRequest)
        {
            return new SuccessfulResponse<object>(await _productService.UpdateProduct(productId, productRequest), (int)HttpStatusCode.OK, "Product modified successfully").GetResponse();
        }
        [HttpPut("productVariant/{productVariantId}")]
        public async Task<IActionResult> UpdateProductVariant(Guid productVariantId, [FromForm] ProductVariantRequest productVariantRequest)
        {
            return new SuccessfulResponse<object>(await _productService.UpdateProductVariant(productVariantId, productVariantRequest), (int)HttpStatusCode.OK, "Product variant modified successfully").GetResponse();
        }
        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteProduct(Guid productId)
        {
            await _productService.DeleteProduct(productId);
            return new SuccessfulResponse<object>(null, (int)HttpStatusCode.OK, "Product deleted successfully").GetResponse();
        }
        [HttpDelete("productVariant/{productVariantId}")]
        public async Task<IActionResult> DeleteProductVariant(Guid productVariantId)
        {
            await _productService.DeleteProductVariant(productVariantId);
            return new SuccessfulResponse<object>(null, (int)HttpStatusCode.OK, "Product variants deleted successfully").GetResponse();
        }
    }
}