using FurnitureStoreBE.Services.UserService;
using FurnitureStoreBE.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using FurnitureStoreBE.DTOs.Request.UserRequest;
using FurnitureStoreBE.Constants;
namespace FurnitureStoreBE.Controllers.User
{
    [ApiController]
    [Route(Routes.USER)]
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
        [HttpGet("address/{userId}")] 
        public async Task<IActionResult> GetUserAddress(string userId)
        {
            return new SuccessfulResponse<object>(await _userService.GetAddressesByUserId(userId), (int)HttpStatusCode.OK, "Get user address successfully").GetResponse();

        }
        [HttpPost("address/{userId}")]
        public async Task<IActionResult> CreateUserAddress(string userId, [FromBody] AddressRequest addressRequest)
        {
            return new SuccessfulResponse<object>(await _userService.CreateUserAddress(userId, addressRequest), (int)HttpStatusCode.Created, "Address has been created successfully").GetResponse();
        }
        [HttpPut("address/{addressId}")]
        public async Task<IActionResult> UpdateUserAddress(Guid addressId, [FromBody] AddressRequest addressRequest)
        {
            return new SuccessfulResponse<object>(await _userService.UpdateUserAddress(addressId, addressRequest), (int)HttpStatusCode.OK, "Address has been modified successfully").GetResponse();
        }
        [HttpDelete("address/{addressId}")]
        public async Task<IActionResult> DeleteUserAddress(Guid addressId)
        {
            await _userService.DeleteUserAddress(addressId);
            return new SuccessfulResponse<object>(null, (int)HttpStatusCode.OK, "Address has been deleted successfully").GetResponse();
        }
    }
}
