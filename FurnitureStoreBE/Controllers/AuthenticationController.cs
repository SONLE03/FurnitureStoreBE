using FurnitureStoreBE.Data;
using FurnitureStoreBE.DTOs.Request.Auth;
using FurnitureStoreBE.Models;
using FurnitureStoreBE.Services.Authentication;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Security.Claims;
using System.Text;

namespace FurnitureStoreBE.Controllers
{
    [ApiController]
    [Route("/auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthService _authenticationService;
        private readonly ApplicationDBContext _applicationDBContext;
        public AuthenticationController(IAuthService authenticationService, ApplicationDBContext applicationDBContext)
        {
            _authenticationService = authenticationService;
            _applicationDBContext = applicationDBContext;
        }
        [HttpPost("/register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest register)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid registration data.");
            }

            try
            {
                await _authenticationService.Register(register);
                return Ok("User created successfully");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("/login")]
        public async Task<IActionResult> Login([FromBody] SigninRequest signinRequest)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid signin data.");
            }

            return Ok(await _authenticationService.Login(signinRequest));
        }

    }
}
