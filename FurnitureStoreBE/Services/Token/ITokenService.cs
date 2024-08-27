using FurnitureStoreBE.Models;
using System;

namespace FurnitureStoreBE.Services.Token
{
    public interface ITokenService
    {
        Task<string> GenerateRefreshToken(User user);
        Task<RefreshToken> FindByToken(string token);
        Task<RefreshToken> VerifyExpiration(RefreshToken refreshToken);
        void DeleteRefreshTokenByUserId(string userId);
    }
}
