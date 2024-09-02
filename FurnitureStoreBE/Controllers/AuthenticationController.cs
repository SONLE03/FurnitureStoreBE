﻿using Azure;
using FurnitureStoreBE.Data;
using FurnitureStoreBE.DTOs.Request.Auth;
using FurnitureStoreBE.DTOs.Request.AuthRequest;
using FurnitureStoreBE.Services.Authentication;
using FurnitureStoreBE.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
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
        [HttpPost("signup")]
        public async Task<IActionResult> Register([FromBody] SignupRequest register)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid registration data.");
            }

            try
            {
                var response = await _authenticationService.Signup(register);
                return new SuccessfulResponse<object>(response, (int)HttpStatusCode.Created, "Signup successfully").GetResponse();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("signin")]
        public async Task<IActionResult> Login([FromBody] SigninRequest loginRequest)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid signin data.");
            }
            return new SuccessfulResponse<object>(await _authenticationService.Signin(loginRequest), (int)HttpStatusCode.OK, "Signin successfully").GetResponse();
        }
        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest tokenRequest)
        {
            return new SuccessfulResponse<object>(await _authenticationService.HandleRefreshToken(tokenRequest), (int)HttpStatusCode.OK, "Refresh token successfully").GetResponse();
        }
        [HttpPost("me")]
        [Authorize]
        public async Task<IActionResult> GetMe([FromBody] string userId)
        {
            return new SuccessfulResponse<object>(await _authenticationService.GetMe(userId), (int)HttpStatusCode.OK, "Get me successfully").GetResponse();
        }
        [HttpPost("signout")]
        [Authorize]
        public async Task<IActionResult> Signout([FromBody] string userId)
        {
            _authenticationService.Signout(userId);
            return new SuccessfulResponse<object>(null, (int)HttpStatusCode.OK, "Signout successfully").GetResponse();
        }
    }
}
