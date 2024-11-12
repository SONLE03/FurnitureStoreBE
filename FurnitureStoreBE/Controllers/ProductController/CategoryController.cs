using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.Constants;
using FurnitureStoreBE.DTOs.Request.ProductRequest;
using FurnitureStoreBE.Services.ProductService.CategoryService;
using FurnitureStoreBE.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FurnitureStoreBE.Controllers.ProductController
{
    [ApiController]
    [Route(Routes.CATEGORY)]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService CategoryService)
        {
            _categoryService = CategoryService;
        }
        [HttpGet()]
        public async Task<IActionResult> GetCategories([FromQuery] PageInfo pageInfo)
        {
            return new SuccessfulResponse<object>(await _categoryService.GetAllCategories(pageInfo), (int)HttpStatusCode.OK, "Get Categories successfully").GetResponse();
        }
        [HttpPost()]
        public async Task<IActionResult> CreateCategory([FromForm] CategoryRequest categoryRequest)
        {
            return new SuccessfulResponse<object>(await _categoryService.CreateCategory(categoryRequest), (int)HttpStatusCode.Created, "Category created successfully").GetResponse();
        }
        [HttpPut("{categoryId}")]
        public async Task<IActionResult> UpdateCategory(Guid categoryId, [FromForm] CategoryRequest categoryRequest)
        {
            return new SuccessfulResponse<object>(await _categoryService.UpdateCategory(categoryId, categoryRequest), (int)HttpStatusCode.OK, "Category modified successfully").GetResponse();
        }
        [HttpPost("image/{categoryId}")]
        public async Task<IActionResult> ChangeCategoryImage(Guid categoryId, [FromForm] IFormFile categoryImage)
        {
            await _categoryService.ChangeCategoryImage(categoryId, categoryImage);
            return new SuccessfulResponse<object>(null, (int)HttpStatusCode.OK, "Category image changed successfully").GetResponse();
        }
        [HttpDelete("{categoryId}")]
        public async Task<IActionResult> DeleteCategory(Guid categoryId)
        {
            await _categoryService.DeleteCategory(categoryId);
            return new SuccessfulResponse<object>(null, (int)HttpStatusCode.OK, "Category deleted successfully").GetResponse();

        }
    }
}
