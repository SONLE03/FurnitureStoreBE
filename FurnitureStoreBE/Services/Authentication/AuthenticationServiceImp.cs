using FurnitureStoreBE.Data;
using FurnitureStoreBE.DTOs.Request;
using FurnitureStoreBE.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FurnitureStoreBE.Services.Authentication
{
    public class AuthenticationServiceImp : IAuthenticationService
    {
        private readonly ApplicationDBContext _context;
        private readonly IConfiguration _configuration;
        public AuthenticationServiceImp(ApplicationDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<string> Register(RegisterRequest register)
        {
            if (register == null) throw new ArgumentNullException(nameof(register));

            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == register.Email);

            if (existingUser != null)
            {
                throw new InvalidOperationException("User with this email already exists.");
            }

            var newUser = new User
            {
                Email = register.Email,
                Password = register.Password, // Consider hashing the password before saving
                Role = Enums.ERole.Customer
            };

            var createdUser = _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            var customer = new Customer
            {
                id = createdUser.Entity.Id
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return createdUser.Entity.Id.ToString();
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
                new Claim("Id", Guid.NewGuid().ToString()),
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
