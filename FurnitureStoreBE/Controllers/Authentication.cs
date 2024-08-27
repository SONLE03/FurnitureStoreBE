using FurnitureStoreBE.Data;
using FurnitureStoreBE.DTOs.Request;
using FurnitureStoreBE.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

            try
            {
                var userId = await _authenticationService.Register(register);
                return Ok(userId);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
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

    }
}
