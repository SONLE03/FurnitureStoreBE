using FurnitureStoreBE.Data;
using FurnitureStoreBE.Services.FileUploadService;
using FurnitureStoreBE.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureStoreBE.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IUserService userService;
        private readonly IFileUploadService _fileUploadService;
        public UserController(ApplicationDBContext context, IUserService userService, IFileUploadService fileUploadService)
        {
            _context = context;
            this.userService = userService;
            _fileUploadService = fileUploadService;
        }
        [HttpGet()]
        //[Authorize(Policy = "CreateUserPolicy")]
        public async Task<IActionResult>  getVoid()
        {
            var result = await userService.GetTypeClaimsByRole("Staff");
            return Ok();
        }
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFiles(List<IFormFile> files, [FromQuery] string folderName = "default_folder")
        {
            if (files.Count == 0)
            {
                return BadRequest("No files uploaded.");
            }

            var uploadResults = await _fileUploadService.UploadFilesAsync(files, folderName);

            return Ok(new { Results = uploadResults });
        }

        [HttpDelete("destroy")]
        public async Task<IActionResult> DestroyFile([FromQuery] string publicId)
        {
            if (string.IsNullOrEmpty(publicId))
            {
                return BadRequest("Public ID is required.");
            }

            var result = await _fileUploadService.DestroyFileAsync(publicId);

            if (result.Result == "ok")
            {
                return Ok(new { Message = "File successfully deleted.", Result = result });
            }
            else
            {
                return NotFound(new { Message = "File not found.", Result = result });
            }
        }
    }
}
