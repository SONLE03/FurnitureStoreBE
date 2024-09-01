using FurnitureStoreBE.Data;
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
        public JwtUtil(ApplicationDBContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            issuer = _configuration["Jwt:Issuer"];
            audience = _configuration["Jwt:Audience"];
            secretKey = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? string.Empty);
        }

        public string GenerateToken(string userId, string userName, string userRole)
        {
          
            var claims = new List<Claim>
            {   
                new Claim("id", userId),
                new Claim(ClaimTypes.Name, userName),   
                new Claim(ClaimTypes.Role, userRole)
            };
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(60),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha512Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var stringToken = tokenHandler.WriteToken(token);
            return tokenHandler.WriteToken(token);
        }
    }
}
