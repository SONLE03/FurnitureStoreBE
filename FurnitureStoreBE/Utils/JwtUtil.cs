using FurnitureStoreBE.Data;
using FurnitureStoreBE.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FurnitureStoreBE.Utils
{
    public class JwtUtil
    {
        private readonly IConfiguration _configuration;
        private string? issuer;
        private string? audience;
        private byte[]? secretKey;
        private double accessTokenExpirationTime;
        public JwtUtil(IConfiguration configuration)
        {
            _configuration = configuration;
            issuer = _configuration["Jwt:Issuer"];
            audience = _configuration["Jwt:Audience"];
            secretKey = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? string.Empty);
            accessTokenExpirationTime = Convert.ToDouble(_configuration["Jwt:AccessTokenExpirationTime"]);
        }

        public async Task<string> GenerateToken(User user, string role, IList<Claim> claims)
        {
            var _claims = new List<Claim>
            {
                new Claim("id", user.Id),
                new Claim(ClaimTypes.NameIdentifier, user.UserName),
                new Claim(ClaimTypes.Role, role)
            };
            _claims.AddRange(claims);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(_claims),
                Expires = DateTime.UtcNow.AddHours(accessTokenExpirationTime),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha512Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var stringToken = tokenHandler.WriteToken(token);
            return await Task.FromResult(stringToken);
        }
    }
}
