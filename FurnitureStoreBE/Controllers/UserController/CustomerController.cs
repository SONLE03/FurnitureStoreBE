using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.Constants;
using FurnitureStoreBE.DTOs.Request.UserRequest;
using FurnitureStoreBE.Enums;
using FurnitureStoreBE.Services.FileUploadService;
using FurnitureStoreBE.Services.UserService;
using FurnitureStoreBE.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FurnitureStoreBE.Controllers.User
{
    [ApiController]
    [Route(Routes.CUSTOMER)]
    public class CustomerController : ControllerBase
    {
        private readonly IUserService _userService;
        public CustomerController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet]
        public async Task<IActionResult> GetCustomer([FromQuery] PageInfo pageInfo)
        {
            return new SuccessfulResponse<object>(await _userService.GetAllUsers(ERole.Customer.ToString(), pageInfo), (int)HttpStatusCode.OK, "Get customer successfully").GetResponse();
        }
        [HttpPost()]
        public async Task<IActionResult> CreateCustomer([FromBody] UserRequestCreate userRequestCreate)
        {
            return new SuccessfulResponse<object>(await _userService.CreateUser(userRequestCreate, ERole.Customer.ToString()), (int)HttpStatusCode.Created, "Customer created successfully").GetResponse();
        }
        [HttpPut("{customerId}")]
        public async Task<IActionResult> UpdateCustomer(string customerId, [FromBody] UserRequestUpdate userRequestUpdate)
        {
            return new SuccessfulResponse<object>(await _userService.UpdateUser(customerId, userRequestUpdate), (int)HttpStatusCode.OK, "Customer modified successfully").GetResponse();
        }
        [HttpPut("ban/{customerId}")]
        public async Task<IActionResult> BanCustomer(string customerId)
        {
            await _userService.BanUser(customerId);
            return new SuccessfulResponse<object>(null, (int)HttpStatusCode.OK, "Customer ban successfully").GetResponse();
        }
        [HttpPut("unban/{customerId}")]
        public async Task<IActionResult> UnbanCustomer(string customerId)
        {
            await _userService.UnbanUser(customerId);
            return new SuccessfulResponse<object>(null, (int)HttpStatusCode.OK, "Customer unban successfully").GetResponse();
        }
        [HttpPut("claims/{customerId}")]
        public async Task<IActionResult> UpdateCustomerClaims(string customerId, [FromBody] List<UserClaimsRequest> userClaimsRequest)
        {
            await _userService.UpdateUserClaims(customerId, userClaimsRequest);
            return new SuccessfulResponse<object>(null, (int)HttpStatusCode.OK, "Customer claims modified successfully").GetResponse();
        }
        [HttpDelete("{customerId}")]
        public async Task<IActionResult> DeleteCustomer(string customerId)
        {
            await _userService.DeleteUser(customerId);
            return new SuccessfulResponse<object>(null, (int)HttpStatusCode.OK, "Customer deleted successfully").GetResponse();

        }
    }
}
