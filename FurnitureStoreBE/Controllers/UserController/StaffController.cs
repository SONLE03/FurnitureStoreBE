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
    [Route(Routes.STAFF)]
    public class StaffController : ControllerBase
    {
        private readonly IUserService _userService;
        public StaffController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet]
        public async Task<IActionResult> GetStaff([FromQuery] PageInfo pageInfo)
        {
            return new SuccessfulResponse<object>(await _userService.GetAllUsers(ERole.Staff.ToString(), pageInfo), (int)HttpStatusCode.OK, "Get staff successfully").GetResponse();
        }
        [HttpGet("claims/{staffId}")]
         public async Task<IActionResult> GetStaffClaimsByStaffId(string staffId)
        {
            return new SuccessfulResponse<object>(await _userService.GetUserClaims(staffId), (int)HttpStatusCode.Created, "Staff claims retrieved successfully").GetResponse();
        }

        [HttpGet("claims")]
        public async Task<IActionResult> GetStaffClaims()
        {
            var result = await _userService.GetClaimsByRole(2);
            return new SuccessfulResponse<object>(result, (int)HttpStatusCode.Created, "Claims retrieved successfully").GetResponse();
        }
        [HttpPost()]
        public async Task<IActionResult> CreateStaff([FromBody] UserRequestCreate userRequestCreate)
        {
            return new SuccessfulResponse<object>(await _userService.CreateUser(userRequestCreate, ERole.Staff.ToString()), (int)HttpStatusCode.Created, "Staff created successfully").GetResponse();
        }
        [HttpPut("{staffId}")]
        public async Task<IActionResult> UpdateStaff(string staffId, [FromBody] UserRequestUpdate userRequestUpdate)
        {
            return new SuccessfulResponse<object>(await _userService.UpdateUser(staffId, userRequestUpdate), (int)HttpStatusCode.OK, "Staff modified successfully").GetResponse();
        }
        [HttpPut("ban/{staffId}")]
        public async Task<IActionResult> BanStaff(string staffId)
        {
            await _userService.BanUser(staffId);
            return new SuccessfulResponse<object>(null, (int)HttpStatusCode.OK, "Staff ban successfully").GetResponse();
        }
        [HttpPut("unban/{staffId}")]
        public async Task<IActionResult> UnbanStaff(string staffId)
        {
            await _userService.UnbanUser(staffId);
            return new SuccessfulResponse<object>(null, (int)HttpStatusCode.OK, "Staff unban successfully").GetResponse();
        }
        [HttpPut("claims/{staffId}")]
        public async Task<IActionResult> UpdateStaffClaims(string staffId, [FromBody] List<UserClaimsRequest> userClaimsRequest)
        {
            await _userService.UpdateUserClaims(staffId, userClaimsRequest);
            return new SuccessfulResponse<object>(null, (int)HttpStatusCode.OK, "Staff claims modified successfully").GetResponse();
        }
        [HttpDelete("{staffId}")]
        public async Task<IActionResult> DeleteStaff(string staffId)
        {
            await _userService.DeleteUser(staffId);
            return new SuccessfulResponse<object>(null, (int)HttpStatusCode.OK, "Staff deleted successfully").GetResponse();

        }
    }
}
