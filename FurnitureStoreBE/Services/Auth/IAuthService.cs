using FurnitureStoreBE.DTOs.Request.Auth;
using FurnitureStoreBE.DTOs.Request.AuthRequest;
using FurnitureStoreBE.DTOs.Response.AuthResponse;
using FurnitureStoreBE.Models;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureStoreBE.Services.Authentication
{
    public interface IAuthService
    {
        Task<bool> Signup(SignupRequest register);
        Task<SigninResponse> Signin(SigninRequest loginRequest);
        Task<User> GetMe(string userId);
        Task<string> HandleRefreshToken(RefreshTokenRequest tokenRequest);
        void Signout(string userId);
    }
}
