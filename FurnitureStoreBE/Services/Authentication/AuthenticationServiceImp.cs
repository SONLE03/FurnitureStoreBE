using FurnitureStoreBE.Data;
using FurnitureStoreBE.DTOs.Request;
using FurnitureStoreBE.Exceptions;
using FurnitureStoreBE.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.AspNetCore.Http;

namespace FurnitureStoreBE.Services.Authentication
{
    public class AuthenticationServiceImp : IAuthenticationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDBContext _context;
        private readonly IConfiguration _configuration;
       

        public AuthenticationServiceImp(ApplicationDBContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string hashedPassword, string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
        public async Task<Guid> GetMe()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue("Id");

            if (Guid.TryParse(userIdClaim, out Guid userId))
            {
                return userId;
            }

            throw new UnauthorizedAccessException("User is not authenticated or ID is missing.");
        }
        public async Task<string> Register(RegisterRequest register)
        {
            if (register == null) throw new ArgumentNullException(nameof(register));

            var existingUser = await _context.Users
                .AnyAsync(u => u.Email == register.Email);

            if (existingUser)
            {
                throw new ObjectAlreadyExistsException("User with this email already exists.");
            }
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var newUser = new User
                {
                    Email = register.Email,
                    Password = HashPassword(register.Password),
                    Role = Enums.ERole.Customer
                };
                var guidString = "3C3E31FD-331B-4269-4B6D-08DCC8603E7F"; // Note: Ensure no extra spaces around hyphens
                if (Guid.TryParse(guidString, out Guid parsedGuid))
                {
                    newUser.setCommonCreate(parsedGuid);
                }
                var createdUser = _context.Users.Add(newUser);
        

                var customer = new Customer
                {
                    id = createdUser.Entity.Id
                };

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return createdUser.Entity.Id.ToString();

            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
           
        }

        static bool AuthenticateNormalUser(string userName, string passWord)
        {
            //Check the given user credential is valid - Usually this should be checked from database
            return userName == "hello@example.com" && passWord == "pass123";
        }
        static bool AuthenticateAdminUser(string userName, string passWord)
        {
            //Check the given user credential is valid - Usually this should be checked from database
            return userName == "admin@example.com" && passWord == "admin123";
        }

        public async Task<string> Login(string username, string password)
        {
            var normalUser = AuthenticateNormalUser(username, password);
            var adminUser = AuthenticateAdminUser(username, password);
            if (!(normalUser || adminUser))
                throw new UnauthorizedAccessException("Invalid credentials");

            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? string.Empty);
            var claims = new List<Claim>()
            {
                new Claim("Id", "123123123"),
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Email, password),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            if (adminUser)
            {
                claims.Add(new Claim(ClaimTypes.Role, "admin"));
            }
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddSeconds(30),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var stringToken = tokenHandler.WriteToken(token);
            return tokenHandler.WriteToken(token);
        }
    }
}
