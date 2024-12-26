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
using FurnitureStoreBE.DTOs.Response.MailResponse;
using FurnitureStoreBE.Services.Caching;
using FurnitureStoreBE.DTOs.Response.UserResponse;
using NuGet.Protocol;
using AutoMapper;

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
        private readonly IRedisCacheService _redisCacheService;
        private readonly IMapper _mapper;

        public AuthServiceImp(ILogger<AuthServiceImp> logger, ApplicationDBContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor
            , JwtUtil jwtUtil, UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager
            , ITokenService tokenService, IMailService mailService, IRedisCacheService redisCacheService, IMapper mapper)
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
            _redisCacheService = redisCacheService;
            _mapper = mapper;
        }
        public async Task<UserResponse> GetMe(string userId)
        {
            if(!await _context.Users.AnyAsync(u => u.Id == userId))
            {
                throw new ObjectNotFoundException("User not found");
            }
            return _mapper.Map<UserResponse>(await _context.Users.Include(a => a.Asset).SingleOrDefaultAsync(u => u.Id == userId));
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
                    UserName = email,
                    Role = defaultRoleRegister,
                    Cart = new Cart()
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
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginRequest.Email);
            if (user == null) throw new ObjectNotFoundException("User not found");
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginRequest.Password, false);
            if (!result.Succeeded) throw new WrongPasswordException();
            if(user.Role != ERole.Customer.ToString())
            {
                var roleClaims = await _context.RoleClaims
                  .Select(rc => new RoleClaimsResponse
                  {
                      Id = rc.Id,
                      ClaimType = rc.ClaimType,
                      RoleId = rc.RoleId,
                      AspNetTypeClaimsId = rc.AspNetTypeClaimsId
                  })
                  .ToListAsync();
                await _redisCacheService.SetData(ERedisKey.roleClaims.ToString(), roleClaims, null);
                var typeClaims = await _context.TypeClaims
                    .Select(tc => new TypeClaimsResponse
                    {
                        Id = tc.Id,
                        Name = tc.Name
                    })
                    .ToListAsync();
                await _redisCacheService.SetData(ERedisKey.typeClaims.ToString(), typeClaims, null);
            }
            return new SigninResponse
            {
                AccessToken = await GenerateAccessToken(user),
                RefreshToken = await _tokenService.GenerateRefreshToken(user),
                UserId = user.Id,
                Role = user.Role,
            }; 
        }


        public async Task<string> GenerateAccessToken(User user)
        {
            var _role = await _userManager.GetRolesAsync(user);
            if (_role is null)
            {
                throw new ObjectNotFoundException("Role not found");
            }
            IList<Claim> claims = await _userManager.GetClaimsAsync(user);
            _logger.LogInformation("Claims for user {UserId}: {Claims}", user.Id, string.Join(", ", claims.Select(c => $"{c.Type}: {c.Value}")));

            var accessToken = await _jwtUtil.GenerateToken(user, _role.First(), claims);
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

        public void Signout(string userId)
        {
            try
            {
                _tokenService.DeleteRefreshTokenByUserId(userId);
                _redisCacheService.RemoveAllData();
            }catch (BusinessException ex)
            {
                throw new BusinessException(ex.Message);
            }
        }

        public async Task<OtpResponse> SendOtp(string email)
        {
            //The function of this method is to send an OTP to the user.If the email is sent successfully. OTP will be stored in Redis Cache in 5 minutes.
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
            await _redisCacheService.SetData(email, randomNumber, TimeSpan.FromMinutes(5));

            await _mailService.SendEmailAsync(mailRequest);

            return new OtpResponse
            {
                Otp = randomNumber,
                Email = email
            };
        }
        public async Task VerifyOtp(OtpRequest request)
        {
            string email = request.Email;
            int otp = request.Otp;
            var data = await _redisCacheService.GetData<int>(email);
            if(otp != data)
            {
                throw new BusinessException("Invalid OTP provided.");
            }
            await _redisCacheService.RemoveData(email);

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
