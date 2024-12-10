using FurnitureStoreBE.DTOs.Request.Auth;
using FurnitureStoreBE.DTOs.Request.AuthRequest;
using FurnitureStoreBE.DTOs.Response.AuthResponse;
using FurnitureStoreBE.DTOs.Response.MailResponse;
using FurnitureStoreBE.DTOs.Response.UserResponse;
using FurnitureStoreBE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace FurnitureStoreBE.Services.Authentication
{
    public interface IAuthService
    {
        Task<bool> Signup(SignupRequest register);
        Task<SigninResponse> Signin(SigninRequest loginRequest);
        Task<UserResponse> GetMe(string userId);
        Task<string> HandleRefreshToken(RefreshTokenRequest tokenRequest);
        void Signout(string userId);
        Task<OtpResponse> SendOtp(string email);
        Task VerifyOtp(OtpRequest otpRequest);
        Task ChangePassword(ChangePasswordRequest changePasswordRequest);
        Task ResetPassword(ResetPasswordRequest resetPasswordRequest);
    }
}
