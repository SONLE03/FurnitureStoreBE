using FurnitureStoreBE.Data;
using FurnitureStoreBE.DTOs.Request;
using FurnitureStoreBE.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;

namespace FurnitureStoreBE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ApplicationDBContext _applicationDBContext;
        public AuthenticationController(IAuthenticationService authenticationService, ApplicationDBContext applicationDBContext)
        {
            _authenticationService = authenticationService;
            _applicationDBContext = applicationDBContext;
        }
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequest register)
        {
            if (register == null)
            {
                return BadRequest("Invalid registration data.");
            }
            var userId = await _authenticationService.Register(register);
            return Ok(userId);
        }
        [HttpPost("login/{email}/{password}")]
        public async Task<IActionResult> Login(string email, string password)
        {
            //var user = await _applicationDBContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            //string key = $"dddddd.{user.FullName}.{user.Id}";
            //string accessToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(key));
            //return Ok(accessToken);
           
            return Ok(await _authenticationService.Login(email, password));
        }
        //[HttpGet("me")]
        //[Authorize]
        //public IActionResult GetMe()
        //{
        //    // Lấy thông tin người dùng từ Claims
        //    var userId = User.FindFirstValue("Id"); // Lấy Id từ claim
        //    var userName = User.Identity.Name; // Lấy tên người dùng từ claim
        //    var email = User.FindFirstValue(ClaimTypes.Email); // Lấy email từ claim
        //    var role = User.FindFirstValue(ClaimTypes.Role); // Lấy vai trò từ claim nếu có

        //    return Ok(new
        //    {
        //        UserId = userId,
        //        UserName = userName,
        //        Email = email,
        //        Role = role
        //    });
        //}
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            try
            {
                var userId = await _authenticationService.GetMe();
                return Ok(userId);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
        }

    }
}
