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
using FurnitureStoreBE.Enums;
using FurnitureStoreBE.Services.Token;
using FurnitureStoreBE.DTOs.Request.AuthRequest;
using FurnitureStoreBE.DTOs.Request.MailRequest;
using System.Xml.Linq;
using FurnitureStoreBE.DTOs.Response.MailResponse;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Org.BouncyCastle.Asn1.Ocsp;

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
        private readonly IMailService _mailService;

        public AuthServiceImp(ILogger<AuthServiceImp> logger, ApplicationDBContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor
            , JwtUtil jwtUtil, UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, ITokenService tokenService, IMailService mailService)
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
            _mailService = mailService;

        }
        public async Task<User> GetMe(string userId)
        {
            if(!await _context.Users.AnyAsync(u => u.Id == userId))
            {
                throw new ObjectNotFoundException("User not found");
            }
            return await _context.Users.FirstAsync(u => u.Id == userId);
        }
        public async Task<bool> Signup(SignupRequest register)
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
        public async Task<SigninResponse> Signin(SigninRequest loginRequest)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginRequest.Email.ToLower());
            if (user == null) throw new ObjectNotFoundException("User not found");
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginRequest.Password, false);
            if (!result.Succeeded) throw new WrongPasswordException();
            return new SigninResponse
            {
                AccessToken = await GenerateAccessToken(user),
                RefreshToken = await _tokenService.GenerateRefreshToken(user),
                UserId = user.Id,
            };
        }


        public async Task<string> GenerateAccessToken(User user)
        {
            var _role = await _userManager.GetRolesAsync(user);
            if (_role == null)
            {
                throw new ObjectNotFoundException("Role not found");
            }
            IList<Claim> claims = await _userManager.GetClaimsAsync(user);
            _logger.LogInformation("Claims for user {UserId}: {Claims}", user.Id, string.Join(", ", claims.Select(c => $"{c.Type}: {c.Value}")));

            var accessToken = await _jwtUtil.GenerateToken(user, _role.FirstOrDefault(), claims);
            return accessToken;
        }

        public async Task<string> HandleRefreshToken(RefreshTokenRequest tokenRequest)
        {
            var userId = tokenRequest.UserId;
            var token = tokenRequest.Token;
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null) throw new ObjectNotFoundException("User not found");
            if (!await _tokenService.FindByToken(userId, token)) throw new ObjectNotFoundException("The user does not contain this token");
            _tokenService.VerifyExpiration(token);
            return await GenerateAccessToken(user);
        }

        public async void Signout(string userId)
        {
            try
            {
                _tokenService.DeleteRefreshTokenByUserId(userId);
            }catch (BusinessException ex)
            {
                throw new BusinessException(ex.Message);
            }
        }

        public async Task<OtpResponse> ForgotPassword(string email)
        {
            if (!await _context.Users.AnyAsync(u => u.Email == email))
            {
                throw new ObjectNotFoundException("User with this email not found.");
            }
            Random random = new Random();
            int randomNumber = random.Next(100000, 1000000);
            var mailRequest = new MailRequest
             {
                 ToEmail = email,
                 Subject = "Verification code to reset password.",
                 Body = $@"Hi! This is the one-time password (OTP): {randomNumber}. Do not share this OTP with anyone."
             };
            await _mailService.SendEmailAsync(mailRequest);
            return new OtpResponse
            {
                Otp = randomNumber,
                Email = email
            };
        }

        public async Task ChangePassword(ChangePasswordRequest changePasswordRequest)
        {
            var user = await _context.Users.Where(u => changePasswordRequest.UserId == u.Id).FirstAsync();
            if(user == null)
            {
                throw new ObjectNotFoundException("User not found");
            }
            IdentityResult changePasswordResult = await _userManager.ChangePasswordAsync(user, changePasswordRequest.OldPassword, changePasswordRequest.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                throw new BusinessException("Change password failed");
            }
        }

        public async Task ResetPassword(ResetPasswordRequest resetPasswordRequest)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordRequest.Email);
            if (user == null)
            {
                throw new ObjectNotFoundException("User not found.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var resultRemovePassword = await _userManager.RemovePasswordAsync(user);
                if (!resultRemovePassword.Succeeded)
                {
                    throw new BusinessException("Failed to remove the current password.");
                }
                var resultResetPassword = await _userManager.AddPasswordAsync(user, resetPasswordRequest.NewPassword);
                if (!resultResetPassword.Succeeded)
                {
                    throw new BusinessException("Failed to set the new password.");
                }
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new BusinessException("Reset password failed");
            }
        }

    }
}
