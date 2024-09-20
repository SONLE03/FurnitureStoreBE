using FurnitureStoreBE.Data;
using FurnitureStoreBE.Exceptions;
using FurnitureStoreBE.Models;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;
using NuGet.Protocol.Core.Types;
using Org.BouncyCastle.Utilities.Net;
using System.Net;
using System.Text;

namespace FurnitureStoreBE.Services.Token
{
    public class TokenServiceImp : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TokenServiceImp(ApplicationDBContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public void DeleteAllTokenByUserId(string userId)
        {
            var refreshTokens = _context.Tokens
               .Where(token => token.User.Id == userId)
               .ToList();
            if (refreshTokens.Any())
            {
                _context.Tokens.RemoveRange(refreshTokens);
                _context.SaveChanges();
            }
        }

        public void DeleteRefreshTokenByUserId(string userId)
        {
            var ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
            var userAgent = _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString();
            var refreshTokens = _context.Tokens
                .Where(token => token.User.Id == userId 
                            && token.IpAddress == ipAddress 
                            && token.UserAgent == userAgent)
                .ToList();
            if (refreshTokens.Any())
            {
                _context.Tokens.RemoveRange(refreshTokens);
                _context.SaveChanges();
            }
        }

        public Task<bool> FindByToken(string userId, string refreshToken) => _context.Tokens.AnyAsync(token => token.Token == refreshToken && token.User.Id == userId);

        public async Task<string> GenerateRefreshToken(User user)
        {
            var refreshTokenExpirationTime = Convert.ToInt16(_configuration["Jwt:RefreshTokenExpirationTime"]);
            var token = Guid.NewGuid().ToString();
            var ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
            var userAgent = _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString();
            var refreshToken = new RefreshToken
            {
                User = user,
                Token = token,
                ExpiredDate = DateTime.UtcNow.AddMonths(refreshTokenExpirationTime),
                IpAddress = ipAddress,
                UserAgent = userAgent
            };
            _context.Tokens.Add(refreshToken);
            await _context.SaveChangesAsync();
            return token;
        }
        public bool VerifyExpiration(string _token)
        {
            var refreshToken = _context.Tokens.Where(token => token.Token == _token).FirstOrDefault();
            if (refreshToken == null)
            {
                throw new ObjectNotFoundException("Token not found");
            } 

            if(refreshToken.ExpiredDate.CompareTo(DateTime.UtcNow) < 0)
            {
                _context.Remove(refreshToken);
                _context.SaveChanges();
                throw new BusinessException(refreshToken.Token + " Refresh token was expired. Please make a new signin request");
            }
            return true;
        }
    }
}
