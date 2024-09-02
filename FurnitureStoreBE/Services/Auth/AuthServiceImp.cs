using FurnitureStoreBE.Data;
using FurnitureStoreBE.Exceptions;
using FurnitureStoreBE.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using FurnitureStoreBE.DTOs.Request.Auth;
using FurnitureStoreBE.DTOs.Response.AuthResponse;
using FurnitureStoreBE.Utils;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Net.WebSockets;
using FurnitureStoreBE.Enums;
using FurnitureStoreBE.Services.Token;

namespace FurnitureStoreBE.Services.Authentication
{
    public class AuthServiceImp : IAuthService
    {
        private readonly ILogger<AuthServiceImp> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDBContext _context;
        private readonly IConfiguration _configuration;
        private readonly JwtUtil _jwtUtil;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;

        public AuthServiceImp(ILogger<AuthServiceImp> logger, ApplicationDBContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor
            , JwtUtil jwtUtil, UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, ITokenService tokenService)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _jwtUtil = jwtUtil;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
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
        public async Task<bool> Register(RegisterRequest register)
        {
            var stopwatch = Stopwatch.StartNew();
            string email = register.Email;
            string password = register.Password;
            string defaultRoleRegister = ERole.Customer.ToString();

            if (await _context.Users.AnyAsync(u => u.Email == email))
            {
                throw new ObjectAlreadyExistsException("User with this email already exists.");
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var newUser = new User
                {
                    Email = email,
                    UserName = email
                };

                var createdUserResult = await _userManager.CreateAsync(newUser, password);
                if (!createdUserResult.Succeeded)
                {
                    throw new BusinessException("Failed to create account.");
                }

                var roleExists = await _roleManager.FindByNameAsync(defaultRoleRegister);
                if (roleExists == null)
                {
                    throw new ObjectNotFoundException($"{defaultRoleRegister} role does not exist.");
                }

                var roleResult = await _userManager.AddToRoleAsync(newUser, defaultRoleRegister);
                if (!roleResult.Succeeded)
                {
                    throw new BusinessException("Failed to assign role to user.");
                }

                var claims = await _roleManager.GetClaimsAsync(roleExists);
                var claimsResult = await _userManager.AddClaimsAsync(newUser, claims);
                if (!claimsResult.Succeeded)
                {
                    throw new BusinessException("Failed to assign claims to user.");
                }

                var customer = new Customer { id = newUser.Id };
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                Console.WriteLine($"Register method executed in: {stopwatch.ElapsedMilliseconds} ms");
            }
        }
        public async Task<LoginResponse> Login(SigninRequest loginRequest)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginRequest.Email.ToLower());
            if (user == null) throw new ObjectNotFoundException("User not found");
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginRequest.Password, false);
            if (!result.Succeeded) throw new WrongPasswordException();
            return await this.GenerateToken(user);
        }
        public async Task<LoginResponse> GenerateToken(User user)
        {
            var _role = await _userManager.GetRolesAsync(user);
            if (_role == null)
            {
                throw new ObjectNotFoundException("Role not found");
            }
            IList<Claim> claims = await _userManager.GetClaimsAsync(user);
            _logger.LogInformation("Claims for user {UserId}: {Claims}", user.Id, string.Join(", ", claims.Select(c => $"{c.Type}: {c.Value}")));

            var accessToken = await _jwtUtil.GenerateToken(user, _role.FirstOrDefault(), claims);
            var refreshToken = await _tokenService.GenerateRefreshToken(user);
            return new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserId = user.Id,
            };
        }
    }
}
