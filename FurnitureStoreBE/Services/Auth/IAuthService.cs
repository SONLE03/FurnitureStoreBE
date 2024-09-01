using FurnitureStoreBE.DTOs.Request.Auth;
using FurnitureStoreBE.DTOs.Response.AuthResponse;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureStoreBE.Services.Authentication
{
    public interface IAuthService
    {
        Task Register(RegisterRequest register);
        Task<LoginResponse> Login(SigninRequest loginRequest);
        Task<Guid> GetMe();
    }
}
