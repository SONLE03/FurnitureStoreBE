using FurnitureStoreBE.Models;
using System;

namespace FurnitureStoreBE.Services.Token
{
    public interface ITokenService
    {
        Task<string> GenerateRefreshToken(User user);
        Task<bool> FindByToken(string userId, string token);
        bool VerifyExpiration(string refreshToken);
        void DeleteRefreshTokenByUserId(string userId);
        void DeleteAllTokenByUserId(string userId);
    }
}
