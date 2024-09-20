using FurnitureStoreBE.Services.UserService;
using FurnitureStoreBE.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Serilog;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Controller;
namespace FurnitureStoreBE.Controllers.User
{
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost("avatar/{userId}")]
        public async Task<IActionResult> ChangeAvatar(string userId, IFormFile avatar)
        {
            await _userService.ChangeAvatar(userId, avatar);
            return new SuccessfulResponse<object>(null, (int)HttpStatusCode.OK, "Avatar changed successfully").GetResponse();
        
    }
}
