using FurnitureStoreBE.DTOs.Request;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureStoreBE.Services.Authentication
{
    public interface IAuthenticationService
    {
        Task<string> Register(RegisterRequest register);
        Task<string> Login(string username, string password);
    }
}
