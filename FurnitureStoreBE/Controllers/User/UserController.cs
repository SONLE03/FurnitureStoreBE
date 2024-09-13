using FurnitureStoreBE.DTOs.Request.UserRequest;
using FurnitureStoreBE.Enums;
using FurnitureStoreBE.Services.UserService;
using FurnitureStoreBE.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FurnitureStoreBE.Controllers.User
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly Dictionary<int, string> _roleService;
        public UserController(IUserService userService, Dictionary<int, string> roleService)
        {
            _userService = userService;
            _roleService = roleService;
            _roleService.Add((int)ERole.Owner, ERole.Owner.ToString());
            _roleService.Add((int)ERole.Staff, ERole.Staff.ToString());
            _roleService.Add((int)ERole.Customer, ERole.Customer.ToString());

        }
        [HttpGet()]
        public async Task<IActionResult> getUsers()
        {
            var result = await _userService.GetClaimsByRole(1);
            return new SuccessfulResponse<object>(result, (int)HttpStatusCode.Created, "Claims retrieved successfully").GetResponse();


        }

        //[HttpPost("upload")]
        //public async Task<IActionResult> UploadFiles(List<IFormFile> files, [FromQuery] string folderName = "default_folder")
        //{
        //    if (files.Count == 0)
        //    {
        //        return BadRequest("No files uploaded.");
        //    }

        //    var uploadResults = await _fileUploadService.UploadFilesAsync(files, folderName);

        //    return Ok(new { Results = uploadResults });
        //}

        //[HttpDelete("destroy")]
        //public async Task<IActionResult> DestroyFile([FromQuery] string publicId)
        //{
        //    if (string.IsNullOrEmpty(publicId))
        //    {
        //        return BadRequest("Public ID is required.");
        //    }

        //    var result = await _fileUploadService.DestroyFileAsync(publicId);

        //    if (result.Result == "ok")
        //    {
        //        return Ok(new { Message = "File successfully deleted.", Result = result });
        //    }
        //    else
        //    {
        //        return NotFound(new { Message = "File not found.", Result = result });
        //    }
        //}
        [HttpPost("post")]
        public async Task<IActionResult> CreateUser([FromBody] UserRequestCreate userRequest)
        {
            return Ok(await _userService.CreateUser(userRequest, ERole.Staff.ToString()));
        }
        [HttpPost("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId, [FromBody] UserRequestUpdate userRequest)
        {
         
            return Ok(await _userService.UpdateUser(userId,userRequest));
        }
    }
}
