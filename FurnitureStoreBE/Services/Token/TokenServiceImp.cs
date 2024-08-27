using FurnitureStoreBE.Models;

namespace FurnitureStoreBE.Services.Token
{
    public class TokenServiceImp : ITokenService
    {

        public void DeleteRefreshTokenByUserId(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<RefreshToken> FindByToken(string token)
        {
            throw new NotImplementedException();
        }

        public Task<string> GenerateRefreshToken(User user)
        {
            throw new NotImplementedException();
        }

        public Task<RefreshToken> VerifyExpiration(RefreshToken refreshToken)
        {
            throw new NotImplementedException();
        }
    }
}
