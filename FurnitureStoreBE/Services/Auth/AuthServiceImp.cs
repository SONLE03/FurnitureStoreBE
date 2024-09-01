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

namespace FurnitureStoreBE.Services.Authentication
{
    public class AuthServiceImp : IAuthService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDBContext _context;
        private readonly IConfiguration _configuration;
        private readonly JwtUtil _jwtUtil;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthServiceImp(ApplicationDBContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor
            , JwtUtil jwtUtil, UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _jwtUtil = jwtUtil;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
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
        public async Task Register(RegisterRequest register)
        {
            var stopwatch = Stopwatch.StartNew();
            string email = register.Email;
            string password = register.Password;
            string defaultRoleRegister = "Customer";

            // Check if the user already exists
            if (await _context.Users.AnyAsync(u => u.Email == email))
            {
                throw new ObjectAlreadyExistsException("User with this email already exists.");
            }

            // Use a transaction to ensure atomicity
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Create new user
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

                // Get role and check existence
                var roleExists = await _roleManager.FindByNameAsync(defaultRoleRegister);
                if (roleExists == null)
                {
                    throw new ObjectNotFoundException($"{defaultRoleRegister} role does not exist.");
                }

                // Assign role and claims
                var roleResult = await _userManager.AddToRoleAsync(newUser, defaultRoleRegister);
                if (!roleResult.Succeeded)
                {
                    throw new BusinessException("Failed to assign role to user.");
                }

                // Retrieve and assign claims
                var claims = await _roleManager.GetClaimsAsync(roleExists);
                var claimsResult = await _userManager.AddClaimsAsync(newUser, claims);
                if (!claimsResult.Succeeded)
                {
                    throw new BusinessException("Failed to assign claims to user.");
                }

                // Add customer record
                var customer = new Customer { id = newUser.Id };
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                // Commit transaction
                await transaction.CommitAsync();
            }
            catch
            {
                // Rollback transaction in case of error
                await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                stopwatch.Stop(); // Stop timing
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
            var role = await _userManager.GetRolesAsync(user);
            var accessToken = _jwtUtil.GenerateToken(user.Id, user.Email, role.FirstOrDefault());

            return new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = accessToken,
                UserId = user.Id,
            };
        }
    }
}
