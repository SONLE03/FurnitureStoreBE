using Azure;
using FurnitureStoreBE.Data;
using FurnitureStoreBE.DTOs.Request.Auth;
using FurnitureStoreBE.Services.Authentication;
using FurnitureStoreBE.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FurnitureStoreBE.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthService _authenticationService;
        private readonly ApplicationDBContext _applicationDBContext;
        public AuthenticationController(IAuthService authenticationService, ApplicationDBContext applicationDBContext)
        {
            _authenticationService = authenticationService;
            _applicationDBContext = applicationDBContext;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest register)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid registration data.");
            }

            try
            {
                var response = await _authenticationService.Register(register);
                return new SuccessfulResponse<object>(response, (int)HttpStatusCode.Created, "Register successfully").GetResponse();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] SigninRequest loginRequest)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid signin data.");
            }

            return Ok(await _authenticationService.Login(loginRequest));
        }
        //[HttpPost("refreshToken")]
        //public async Task<IActionResult> RefreshToken(HttpRequest httpRequest)
        //{
        //    string token = httpRequest.
        //    return Ok();
        //}
    }
}
